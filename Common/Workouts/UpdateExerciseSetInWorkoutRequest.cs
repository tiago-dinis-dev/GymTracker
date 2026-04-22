using System.ComponentModel.DataAnnotations;

namespace Common.Workouts;

public record UpdateExerciseSetInWorkoutRequest
{
    [Required(ErrorMessage = "Set index is required.")]
    public required int SetIndex { get; init; }
    public int? Reps { get; init; }
    public float? Weight {  get; init; }
    public decimal? Estimated1Rm { get; init; } 
}
