using Common.AI.Models;
using Common.Exercises;
using Infrastructure.AI.Persistence.InMemory;

namespace Infrastructure.Tests.AI.Persistence;

public class InMemoryMuscleGroupStatsStoreTests
{
    [Fact]
    public async Task UpsertAndGet_ReturnsSameStats()
    {
        var store = new InMemoryMuscleGroupStatsStore();
        var userId = Guid.NewGuid();
        var stats = new MuscleGroupStats { UserId = userId, MuscleGroup = MuscleGroup.Chest, TotalExercises = 3 };

        await store.UpsertAsync(stats, CancellationToken.None);
        var result = await store.GetByUserAndMuscleGroupAsync(userId, MuscleGroup.Chest, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result!.TotalExercises);
    }

    [Fact]
    public async Task Get_NonExistent_ReturnsNull()
    {
        var store = new InMemoryMuscleGroupStatsStore();

        var result = await store.GetByUserAndMuscleGroupAsync(Guid.NewGuid(), MuscleGroup.Back, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DifferentMuscleGroups_StoredSeparately()
    {
        var store = new InMemoryMuscleGroupStatsStore();
        var userId = Guid.NewGuid();

        await store.UpsertAsync(new MuscleGroupStats { UserId = userId, MuscleGroup = MuscleGroup.Chest, TotalExercises = 3 }, CancellationToken.None);
        await store.UpsertAsync(new MuscleGroupStats { UserId = userId, MuscleGroup = MuscleGroup.Back, TotalExercises = 5 }, CancellationToken.None);

        var chest = await store.GetByUserAndMuscleGroupAsync(userId, MuscleGroup.Chest, CancellationToken.None);
        var back = await store.GetByUserAndMuscleGroupAsync(userId, MuscleGroup.Back, CancellationToken.None);

        Assert.Equal(3, chest!.TotalExercises);
        Assert.Equal(5, back!.TotalExercises);
    }
}
