using Application.Common.Interfaces;
using Domain.Common;
using Domain.Exercises;
using Domain.Users;
using Domain.Workouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistence;

public class GymTrackerDbContext(DbContextOptions<GymTrackerDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<Exercise> Exercises => Set<Exercise>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymTrackerDbContext).Assembly);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregates = ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count != 0)
            .ToList();

        var events = aggregates
            .SelectMany(e => e.DomainEvents)
            .ToList();

        if (events.Count == 0)
        {
            return;
        }

        var dispatcher = this.GetService<IDomainEventDispatcher>();
        if (dispatcher != null)
            await dispatcher.DispatchAsync(events, cancellationToken);

        aggregates.ForEach(e => e.ClearDomainEvents());
    }
}
