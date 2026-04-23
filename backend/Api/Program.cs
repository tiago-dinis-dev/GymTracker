using Api.Middleware;
using Api.Services;
using Application;
using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Workouts.Validators;
using FluentValidation;
using Infrastructure;
using Infrastructure.AI.Persistence;
using Infrastructure.Caching;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddValidatorsFromAssemblyContaining<CreateWorkoutRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddExerciseRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below."
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

var connectionStringAI = builder.Configuration.GetConnectionString("DefaultConnectionAI");
if (string.IsNullOrEmpty(connectionStringAI))
{
    throw new InvalidOperationException("Connection string 'DefaultConnectionAI' is not configured.");
}

builder.Services.AddInfrastructure(connectionString, connectionStringAI);
builder.Services.Configure<Infrastructure.AI.Agent.AgentOptions>(
builder.Configuration.GetSection(Infrastructure.AI.Agent.AgentOptions.SectionName));
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>(); 
builder.Services.AddApplication();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT key is not configured.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };

        // If Authorization header is not present, allow reading token from cookie named "X-Access-Token"
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (string.IsNullOrEmpty(ctx.Token) && ctx.Request.Cookies.TryGetValue("X-Access-Token", out var cookieToken))
                {
                    // support both raw token and "Bearer <token>"
                    if (cookieToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        ctx.Token = cookieToken.Substring("Bearer ".Length);
                    else
                        ctx.Token = cookieToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var svc = scope.ServiceProvider;
    var gym = svc.GetRequiredService<GymTrackerDbContext>();

    await gym.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(gym);

    var aiDb = svc.GetRequiredService<AIObservationDbContext>();
    await aiDb.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
