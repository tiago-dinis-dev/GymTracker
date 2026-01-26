using Application.AI.Abstractions;
using Application.Common.Interfaces.Repository;
using Application.Exceptions;
using Common.AI.Observations;
using Domain.Common.Events;

namespace Application.AI.Handlers;

public sealed class ExerciseAddedToWorkoutHandler(IAIObservationQueue aiObservationQueue, IExerciseRepository exerciseRepository)
{
    private readonly IAIObservationQueue _queue = aiObservationQueue;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;

    public async Task HandleAsync(ExerciseAddedToWorkout evt, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(evt.exercise.ExerciseId) ?? throw new NotFoundException("Exercise not found");
        var observation = new ExerciseAddedObservation
        {
            WorkoutId = evt.WorkoutId,
            UserId = evt.userId,
            Metadata = new ExerciseDetails
            {
                Name = exercise.Name,
                Sets = evt.exercise.Sets,
                MuscleGroup = exercise.MuscleGroup,
            },
            VolumeData = (decimal)evt.exercise.CalculateVolume(),
            OrderInWorkout = evt.orderInWorkout,
            Timestamp = evt.OccurredAt,
        };

        await _queue.EnqueueAsync(observation, cancellationToken);
    }
}
