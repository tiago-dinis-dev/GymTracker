using Application.AI.Abstractions;
using Application.Common.Interfaces.Store;
using Common.AI.Models;
using Common.AI.Observations;
using Infrastructure.AI.Calculators;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs;

public class AIObservationBackgroundService(IAIObservationQueue aiObservationQueue, IAIObservationStore aIObservationStore, 
    ILogger<AIObservationBackgroundService> logger, IWorkoutStatsStore workoutStatsStore, IExerciseStatsStore exerciseStatsStore) : BackgroundService
{
    private readonly IAIObservationQueue _aiObservationQueue = aiObservationQueue;
    private readonly IAIObservationStore _aIObservationStore = aIObservationStore;
    private readonly IWorkoutStatsStore _workoutStatsStore = workoutStatsStore;
    private readonly IExerciseStatsStore _exerciseStatsStore = exerciseStatsStore;
    private readonly ILogger<AIObservationBackgroundService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var observation = await _aiObservationQueue.DequeueAsync(stoppingToken);

                switch (observation)
                {
                    case WorkoutCompletedObservation wco:
                        await _aIObservationStore.AddAsync(wco, stoppingToken);

                        var currentStats = await _workoutStatsStore.GetByUserIdAsync(wco.UserId, stoppingToken) 
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

                        await _workoutStatsStore.UpsertAsync(updatedStats, stoppingToken);
                        break;

                    case ExerciseAddedObservation eao:
                        await _aIObservationStore.AddAsync(eao, stoppingToken);

                        var currentExerciseStats = await _exerciseStatsStore.GetByUserIdAndExerciseNameAsync(eao.UserId, eao.Metadata?.Name ?? string.Empty, stoppingToken);
                        var updatedExerciseStats = ExerciseStatsCalculator.Update(currentExerciseStats, eao);

                        await _exerciseStatsStore.UpsertAsync(updatedExerciseStats, stoppingToken);
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
