using Application.AI.Abstractions;
using Common.AI.Observations;
using Domain.Common.Events;

namespace Application.AI.Handlers;

public sealed class WorkoutCompletedHandler(IAIObservationQueue aiObservationQueue)
{
    private readonly IAIObservationQueue _queue = aiObservationQueue;

    public async Task HandleAsync(WorkoutCompleted evt, CancellationToken cancellationToken)
    {
        var observation = new WorkoutCompletedObservation
        {
            WorkoutId = evt.WorkoutId,
            UserId = evt.UserId,
            Timestamp = evt.OccurredAt,
            TotalVolume = (decimal)evt.Exercises.Sum(s => s.CalculateVolume()),
            TotalDuration = evt.OccurredAt - evt.StartedAt,
            ExerciseCount = evt.Exercises.Count,
        };

        await _queue.EnqueueAsync(observation, cancellationToken);
    }
}
