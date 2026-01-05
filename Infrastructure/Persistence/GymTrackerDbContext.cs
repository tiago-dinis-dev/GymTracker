using Domain.Exercises;
using Domain.Users;
using Domain.Workouts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class GymTrackerDbContext(DbContextOptions<GymTrackerDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<Exercise> Exercises => Set<Exercise>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GymTrackerDbContext).Assembly);
    }
}
