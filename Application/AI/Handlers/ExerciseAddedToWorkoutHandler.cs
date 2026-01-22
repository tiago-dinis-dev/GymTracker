using Application.AI.Abstractions;
using Application.Common.Interfaces;
using Application.Exceptions;
using Common.AI.Observations;
using Domain.Common.Events;

namespace Application.AI.Handlers;

public sealed class ExerciseAddedToWorkoutHandler(IAiObservationStore<ExerciseAddedObservation> aiObservationStore, IExerciseRepository exerciseRepository)
{
    private readonly IAiObservationStore<ExerciseAddedObservation> _store = aiObservationStore;
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

        await _store.AddAsync(observation, cancellationToken);
    }
}
