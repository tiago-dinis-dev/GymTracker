using Common.Workouts;
using FluentValidation;

namespace Application.Workouts.Validators;

public class AddExerciseRequestValidator : AbstractValidator<AddExerciseRequest>
{
    public AddExerciseRequestValidator()
    {
        RuleFor(x => x.ExerciseId)
            .NotEmpty().WithMessage("ExerciseId is required.");
        RuleFor(x => x.Sets)
            .NotEmpty().WithMessage("Sets cannot be empty.")
            .ForEach(set =>
            {
                set.Must(s => s.Reps > 0)
                    .WithMessage("Reps must be greater than 0.");
                set.Must(s => s.Weight >= 0)
                    .WithMessage("Weight must be greater than or equal to 0.");
            });
    }
}
