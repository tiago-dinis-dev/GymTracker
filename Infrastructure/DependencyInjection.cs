using Application.AI.Abstractions;
using Application.Common.Interfaces.Repository;
using Application.Common.Interfaces.Store;
using Infrastructure.AI.Persistence;
using Infrastructure.AI.Persistence.InMemory;
using Infrastructure.AI.Queue;
using Infrastructure.BackgroundJobs;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        string connectionString, string connectionStringAI)
    {
        services.AddDbContext<GymTrackerDbContext>(options =>
            options.UseSqlite(connectionString));
        services.AddDbContext<AIObservationDbContext>(options =>
            options.UseSqlite(connectionStringAI));

        services.AddHostedService<WorkoutAutoCompletionService>();
        services.AddHostedService<AIObservationBackgroundService>();

        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IAIObservationStore, AIObservationStore>();

        services.AddSingleton<IWorkoutStatsStore, InMemoryWorkoutStatsStore>();
        services.AddSingleton<IExerciseStatsStore, InMemoryExerciseStatsStore>();
        services.AddSingleton<IAIObservationQueue, InMemoryAIObservationQueue>();

        return services;
    }
}
