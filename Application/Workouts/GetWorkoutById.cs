using Application.Common.Interfaces.Repository;
using Application.Dtos;
using Application.Exceptions;
using Domain.Workouts;

namespace Application.Workouts;

public record GetWorkoutByIdQuery(Guid WorkoutId);

public class GetWorkoutByIdHandler(IWorkoutRepository workoutRepository)
{
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;

    public async Task<WorkoutDetailsDto> HandleAsync(GetWorkoutByIdQuery command)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(command.WorkoutId) ?? throw new NotFoundException("Workout not found.");
        return MapToDto(workout);
    }

    private static WorkoutDetailsDto MapToDto(Workout workout)
    {
        return new WorkoutDetailsDto
        {
            Id = workout.Id,
            UserId = workout.UserId,
            Date = workout.Date,
            Status = workout.Status.ToString(),
            Exercises = [.. workout.Exercises.Select(e => new ExercisePerformedDto
            {
                ExerciseId = e.ExerciseId,
                Sets = [.. e.Sets.Select(s => new SetRecordDto
                {
                    Reps = s.Reps,
                    Weight = s.Weight
                })]
            })]
        };
    }
}
