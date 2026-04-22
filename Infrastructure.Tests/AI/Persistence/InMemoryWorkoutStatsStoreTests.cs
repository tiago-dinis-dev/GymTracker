using Common.AI.Models;
using Infrastructure.AI.Persistence.InMemory;

namespace Infrastructure.Tests.AI.Persistence;

public class InMemoryWorkoutStatsStoreTests
{
    [Fact]
    public async Task UpsertAndGet_ReturnsSameStats()
    {
        var store = new InMemoryWorkoutStatsStore();
        var userId = Guid.NewGuid();
        var stats = new UserWorkoutStats(userId, 1, 5, 5000m, 5000m, TimeSpan.FromMinutes(60), DateTime.UtcNow);

        await store.UpsertAsync(stats, CancellationToken.None);
        var result = await store.GetByUserIdAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result!.TotalWorkouts);
    }

    [Fact]
    public async Task Get_NonExistent_ReturnsNull()
    {
        var store = new InMemoryWorkoutStatsStore();

        var result = await store.GetByUserIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Upsert_OverwritesExistingEntry()
    {
        var store = new InMemoryWorkoutStatsStore();
        var userId = Guid.NewGuid();
        var original = new UserWorkoutStats(userId, 1, 5, 5000m, 5000m, TimeSpan.FromMinutes(60), DateTime.UtcNow);
        var updated = new UserWorkoutStats(userId, 2, 10, 10000m, 5000m, TimeSpan.FromMinutes(55), DateTime.UtcNow);

        await store.UpsertAsync(original, CancellationToken.None);
        await store.UpsertAsync(updated, CancellationToken.None);
        var result = await store.GetByUserIdAsync(userId, CancellationToken.None);

        Assert.Equal(2, result!.TotalWorkouts);
    }
}
