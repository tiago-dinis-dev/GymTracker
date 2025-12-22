using Domain.Exercises;
using Domain.Users;
using Domain.Workouts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class GymTrackerDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<Exercise> Exercises => Set<Exercise>();

    public GymTrackerDbContext(DbContextOptions<GymTrackerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymTrackerDbContext).Assembly);
    }
}
