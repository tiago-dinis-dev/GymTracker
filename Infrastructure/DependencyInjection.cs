using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<GymTrackerDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();

        return services;
    }
}
