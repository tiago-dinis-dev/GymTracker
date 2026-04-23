using Common.Exercises;
using Domain.Exercises;

namespace Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(GymTrackerDbContext context)
    {
        var exercisesToSeed = new[]
        {
            new Exercise("Bench Press", MuscleGroup.Chest, "Intermediate", "Compound chest press using barbell. Primary chest builder."),
            new Exercise("Incline Dumbbell Press", MuscleGroup.Chest, "Intermediate", "Targets upper chest with dumbbells on incline bench."),
            new Exercise("Cable Fly", MuscleGroup.Chest, "Beginner", "Isolation exercise for chest using cable machine."),
            new Exercise("Dips", MuscleGroup.Chest, "Intermediate", "Bodyweight exercise targeting lower chest and triceps."),
            new Exercise("Deadlift", MuscleGroup.Legs, "Advanced", "King of all exercises. Full posterior chain compound movement."),
            new Exercise("Pull-ups", MuscleGroup.Back, "Intermediate", "Bodyweight vertical pull. Builds wide, strong lats."),
            new Exercise("Barbell Row", MuscleGroup.Back, "Intermediate", "Horizontal barbell pull for thick back development."),
            new Exercise("Face Pulls", MuscleGroup.Back, "Beginner", "Cable exercise for rear delts and rotator cuff health."),
            new Exercise("Squat", MuscleGroup.Legs, "Advanced", "The ultimate leg builder. Full lower body compound movement."),
            new Exercise("Romanian Deadlift", MuscleGroup.Legs, "Intermediate", "Hip hinge movement targeting hamstrings and glutes."),
            new Exercise("Leg Press", MuscleGroup.Legs, "Beginner", "Machine compound movement for overall leg development."),
            new Exercise("Leg Extension", MuscleGroup.Legs, "Beginner", "Isolation exercise targeting the quadriceps."),
            new Exercise("Calf Raises", MuscleGroup.Legs, "Beginner", "Isolation movement for calf muscle development."),
            new Exercise("Shoulder Press", MuscleGroup.Shoulders, "Intermediate", "Overhead press for overall shoulder development."),
            new Exercise("Lateral Raises", MuscleGroup.Shoulders, "Beginner", "Isolation exercise for lateral deltoids."),
            new Exercise("Arnold Press", MuscleGroup.Shoulders, "Intermediate", "Full shoulder development with rotation component."),
            new Exercise("Bicep Curls", MuscleGroup.Arms, "Beginner", "Classic curl for bicep peak development."),
            new Exercise("Hammer Curls", MuscleGroup.Arms, "Beginner", "Neutral grip curl for brachialis and forearm development."),
            new Exercise("Tricep Pushdown", MuscleGroup.Arms, "Beginner", "Cable exercise for tricep isolation and definition."),
            new Exercise("Skull Crushers", MuscleGroup.Arms, "Intermediate", "Lying EZ bar extension for tricep mass."),
            new Exercise("Plank", MuscleGroup.Abs, "Beginner", "Isometric core exercise for stability and endurance."),
            new Exercise("Cable Crunch", MuscleGroup.Abs, "Beginner", "Weighted crunch using cable machine for core strength."),
            new Exercise("Hanging Leg Raise", MuscleGroup.Abs, "Intermediate", "Lower ab exercise while hanging from pull-up bar."),
            new Exercise("Ab Wheel Rollout", MuscleGroup.Abs, "Advanced", "Advanced core exercise targeting entire abdominal wall.")
        };

        var existingNames = context.Exercises.Select(e => e.Name).ToHashSet();

        foreach (var ex in exercisesToSeed.Where(ex => !existingNames.Contains(ex.Name)))
        {
            context.Exercises.Add(ex);
        }

        await context.SaveChangesAsync();
    }
}
