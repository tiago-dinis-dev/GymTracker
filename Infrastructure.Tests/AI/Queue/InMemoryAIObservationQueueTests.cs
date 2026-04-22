using Infrastructure.AI.Queue;

namespace Infrastructure.Tests.AI.Queue;

public class InMemoryAIObservationQueueTests
{
    [Fact]
    public async Task EnqueueAndDequeue_ReturnsSameObject()
    {
        var queue = new InMemoryAIObservationQueue();
        var item = new { Name = "test" };

        await queue.EnqueueAsync(item, CancellationToken.None);
        var result = await queue.DequeueAsync(CancellationToken.None);

        Assert.Same(item, result);
    }

    [Fact]
    public async Task Dequeue_MaintainsFifoOrder()
    {
        var queue = new InMemoryAIObservationQueue();
        await queue.EnqueueAsync("first", CancellationToken.None);
        await queue.EnqueueAsync("second", CancellationToken.None);

        var r1 = await queue.DequeueAsync(CancellationToken.None);
        var r2 = await queue.DequeueAsync(CancellationToken.None);

        Assert.Equal("first", r1);
        Assert.Equal("second", r2);
    }

    [Fact]
    public async Task Dequeue_EmptyQueue_BlocksUntilCancelled()
    {
        var queue = new InMemoryAIObservationQueue();
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAsync<OperationCanceledException>(() => queue.DequeueAsync(cts.Token).AsTask());
    }
}
