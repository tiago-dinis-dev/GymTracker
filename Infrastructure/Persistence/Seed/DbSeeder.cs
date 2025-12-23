using Domain.Exercises;

namespace Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(GymTrackerDbContext context)
    {
        if (context.Exercises.Any())
            return;

        var exercises = new[]
        {
            new Exercise("Bench Press", MuscleGroup.Chest),
            new Exercise("Squat", MuscleGroup.Legs),
            new Exercise("Deadlift", MuscleGroup.Legs),
            new Exercise("Overhead Press", MuscleGroup.Shoulders),
            new Exercise("Barbell Row", MuscleGroup.Back),
            new Exercise("Bicep Curl", MuscleGroup.Arms),
            new Exercise("Tricep Push-down", MuscleGroup.Arms),
            new Exercise("Cable Crunches", MuscleGroup.Abs)
        };

        context.Exercises.AddRange(exercises);
        await context.SaveChangesAsync();
    }
}
