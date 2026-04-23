using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository;
using Application.Dtos;

namespace Application.Workouts;

public class GetInProgressWorkoutHandler(IWorkoutRepository workoutRepository, IUserContextService userContextService)
{
    private readonly IWorkoutRepository _workoutRepository = workoutRepository;
    private readonly IUserContextService _userContextService = userContextService;

    public async Task<WorkoutDetailsDto?> HandleAsync()
    {
        var userId = _userContextService.GetUserId();
        var workout = await _workoutRepository.GetInProgressWorkoutAsync(userId);
        if (workout is null) return null;

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
                    Weight = s.Weight,
                    IntensityPercent1Rm = s.IntensityPercent1Rm
                })]
            })]
        };
    }
}
