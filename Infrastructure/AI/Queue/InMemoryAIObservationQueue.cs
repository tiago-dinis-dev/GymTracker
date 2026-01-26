using Application.AI.Abstractions;
using System.Threading.Channels;

namespace Infrastructure.AI.Queue;

public class InMemoryAIObservationQueue() : IAIObservationQueue
{
    private readonly Channel<object> _channel = Channel.CreateUnbounded<object>();
    public ValueTask<object> DequeueAsync(CancellationToken ct)
    {
        return _channel.Reader.ReadAsync(ct);
    }

    public ValueTask EnqueueAsync(object observation, CancellationToken ct)
    {
        return _channel.Writer.WriteAsync(observation, ct);
    }
}
