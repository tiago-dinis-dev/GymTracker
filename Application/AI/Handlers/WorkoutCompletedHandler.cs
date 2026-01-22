using Application.AI.Abstractions;
using Common.AI.Observations;
using Domain.Common.Events;

namespace Application.AI.Handlers;

public sealed class WorkoutCompletedHandler(IAiObservationStore<WorkoutCompletedObservation> aiObservationStore)
{
    private readonly IAiObservationStore<WorkoutCompletedObservation> _store = aiObservationStore;

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

        await _store.AddAsync(observation, cancellationToken);
    }
}
