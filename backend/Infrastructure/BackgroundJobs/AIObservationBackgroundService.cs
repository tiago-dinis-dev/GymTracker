using Application.AI.Abstractions;
using Application.Common.Interfaces.Store;
using Common.AI.Models;
using Common.AI.Observations;
using Infrastructure.AI.Calculators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class AIObservationBackgroundService(IAIObservationQueue aiObservationQueue, IServiceProvider serviceProvider, 
    ILogger<AIObservationBackgroundService> logger) : BackgroundService
{
    private readonly IAIObservationQueue _aiObservationQueue = aiObservationQueue;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<AIObservationBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var observation = await _aiObservationQueue.DequeueAsync(stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var store = scope.ServiceProvider.GetRequiredService<IAIObservationStore>();
                var workoutStatsStore = scope.ServiceProvider.GetRequiredService<IWorkoutStatsStore>();
                var exerciseStatsStore = scope.ServiceProvider.GetRequiredService<IExerciseStatsStore>();
                var muscleGroupStatsStore = scope.ServiceProvider.GetRequiredService<IMuscleGroupStatsStore>();

                switch (observation)
                {
                    case WorkoutCompletedObservation wco:
                        var currentStats = await workoutStatsStore.GetByUserIdAsync(wco.UserId, stoppingToken) 
                            ?? new UserWorkoutStats(
                               UserId: wco.UserId,
                               TotalWorkouts: 0,
                               TotalExercises: 0,
                               TotalVolume: 0m,
                               AverageVolumePerWorkout: 0m,
                               AverageWorkoutDuration: TimeSpan.Zero,
                               LastWorkoutAt: DateTime.MinValue
                            );
                        var updatedStats = WorkoutStatsCalculator.Update(currentStats, wco);

                        await store.AddAsync(wco, stoppingToken);
                        await workoutStatsStore.UpsertAsync(updatedStats, stoppingToken);
                        break;

                    case ExerciseAddedObservation eao:
                        var currentExerciseStats = await exerciseStatsStore.GetByUserIdAndExerciseNameAsync(eao.UserId, eao.Metadata?.Name ?? string.Empty, stoppingToken);
                        var updatedExerciseStats = ExerciseStatsCalculator.Update(currentExerciseStats, eao);

                        var currentMuscleGroupStats = await muscleGroupStatsStore.GetByUserAndMuscleGroupAsync(eao.UserId, eao.MuscleGroup, stoppingToken);
                        var updatedMuscleGroupStats = MuscleGroupStatsCalculator.Update(currentMuscleGroupStats, eao);

                        await store.AddAsync(eao, stoppingToken);
                        await exerciseStatsStore.UpsertAsync(updatedExerciseStats, stoppingToken);
                        await muscleGroupStatsStore.UpsertAsync(updatedMuscleGroupStats, stoppingToken);

                        break;

                    default:
                        _logger.LogWarning("Received unknown observation type: {ObservationType}", observation?.GetType().FullName);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI observation");
            }
        }
    }
}
