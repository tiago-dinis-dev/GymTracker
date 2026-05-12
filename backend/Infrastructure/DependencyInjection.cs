using Application.AI.Abstractions;
using Application.Common.Interfaces.Repository;
using Application.Common.Interfaces.Store;
using Infrastructure.AI.Agent;
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
            options.UseNpgsql(connectionString));
        services.AddDbContext<AIObservationDbContext>(options =>
            options.UseNpgsql(connectionStringAI));

        services.AddHostedService<WorkoutAutoCompletionService>();
        services.AddHostedService<AIObservationBackgroundService>();

        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAIObservationStore, AIObservationStore>();
        services.AddScoped<IWorkoutHistoryQuery, WorkoutHistoryQuery>();

        // Persist derived stats from observations using the AI observation DB
        services.AddScoped<IWorkoutStatsStore, WorkoutStatsDbStore>();
        services.AddScoped<IExerciseStatsStore, ExerciseStatsDbStore>();
        services.AddScoped<IMuscleGroupStatsStore, MuscleGroupStatsDbStore>();
        services.AddSingleton<IAIObservationQueue, InMemoryAIObservationQueue>();

        services.AddScoped<IFitnessAgentService, FitnessAgentService>();

        return services;
    }
}
