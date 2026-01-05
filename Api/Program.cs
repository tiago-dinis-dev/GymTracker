using Api.Middleware;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Infrastructure.Caching;
using Application.Common.Caching;
using Application.Workouts.Validators;
using Common.Services;
using Api.Services;

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

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>(); 
builder.Services.AddApplication();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();

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

app.UseAuthorization();

app.UseCors();

app.MapControllers();

await app.RunAsync();
