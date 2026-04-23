using Common.AI.Observations;

namespace Application.AI.Abstractions;

public interface IAIObservationStore
{
    Task AddAsync(WorkoutCompletedObservation observation, CancellationToken cancellationToken);
    Task AddAsync(ExerciseAddedObservation observation, CancellationToken cancellationToken);
}
