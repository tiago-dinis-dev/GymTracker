using Common.AI.Models;
using Common.Exercises;
using Infrastructure.AI.Persistence.InMemory;

namespace Infrastructure.Tests.AI.Persistence;

public class InMemoryExerciseStatsStoreTests
{
    [Fact]
    public async Task UpsertAndGet_ReturnsSameStats()
    {
        var store = new InMemoryExerciseStatsStore();
        var stats = new ExerciseStats { UserId = Guid.NewGuid(), ExerciseName = "Bench Press", MuscleGroup = MuscleGroup.Chest, TotalSets = 3 };

        await store.UpsertAsync(stats, CancellationToken.None);
        var result = await store.GetByUserIdAndExerciseNameAsync(stats.UserId, "Bench Press", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result!.TotalSets);
    }

    [Fact]
    public async Task Get_NonExistent_ReturnsNull()
    {
        var store = new InMemoryExerciseStatsStore();

        var result = await store.GetByUserIdAndExerciseNameAsync(Guid.NewGuid(), "Nope", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Upsert_OverwritesExistingEntry()
    {
        var store = new InMemoryExerciseStatsStore();
        var userId = Guid.NewGuid();
        var original = new ExerciseStats { UserId = userId, ExerciseName = "Bench Press", MuscleGroup = MuscleGroup.Chest, TotalSets = 3 };
        var updated = original with { TotalSets = 5 };

        await store.UpsertAsync(original, CancellationToken.None);
        await store.UpsertAsync(updated, CancellationToken.None);
        var result = await store.GetByUserIdAndExerciseNameAsync(userId, "Bench Press", CancellationToken.None);

        Assert.Equal(5, result!.TotalSets);
    }
}
