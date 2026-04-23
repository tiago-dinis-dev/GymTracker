namespace Application.AI.Abstractions;

public interface IAIObservationQueue
{
    ValueTask EnqueueAsync(object observation, CancellationToken ct);
    ValueTask<object> DequeueAsync(CancellationToken ct);
}
