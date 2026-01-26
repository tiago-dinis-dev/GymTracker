using Api.Middleware;
using Api.Services;
using Application;
using Application.Common.Caching;
using Application.Common.Interfaces;
using Application.Workouts.Validators;
using FluentValidation;
using Infrastructure;
using Infrastructure.Caching;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<CreateWorkoutRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddExerciseRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>(); 
builder.Services.AddApplication();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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
    var context = scope.ServiceProvider
                       .GetRequiredService<GymTrackerDbContext>();

    await context.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapControllers();

await app.RunAsync();
