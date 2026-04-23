using Application.Common;
using Application.Common.Interfaces;
using Domain.Common.Events;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Application.Tests.Common;

public class DomainEventDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_CallsRegisteredHandler()
    {
        var handler = new Mock<IDomainEventHandler<TestDomainEvent>>();
        var services = new ServiceCollection();
        services.AddSingleton(typeof(IDomainEventHandler<TestDomainEvent>), handler.Object);
        var provider = services.BuildServiceProvider();

        var dispatcher = new DomainEventDispatcher(provider);
        var @event = new TestDomainEvent();

        await dispatcher.DispatchAsync([@event]);

        handler.Verify(x => x.HandleAsync(@event, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_NoHandlers_DoesNotThrow()
    {
        var provider = new ServiceCollection().BuildServiceProvider();
        var dispatcher = new DomainEventDispatcher(provider);

        await dispatcher.DispatchAsync([new TestDomainEvent()]);
    }

    public record TestDomainEvent(DateTime OccurredAt) : IDomainEvent
    {
        public TestDomainEvent() : this(DateTime.UtcNow) { }
    }
}
