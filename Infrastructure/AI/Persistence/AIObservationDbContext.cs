using Common.AI.Observations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.AI.Persistence;

public class AIObservationDbContext(DbContextOptions<AIObservationDbContext> options) : DbContext(options)
{
    public DbSet<WorkoutCompletedObservation> WorkoutCompletedObservations { get; set; } = null!;
    public DbSet<ExerciseAddedObservation> ExerciseAddedObservations { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkoutCompletedObservation>()
            .ToTable("WorkoutCompletedObservations")
            .HasKey(e => new { e.WorkoutId, e.Timestamp });

        modelBuilder.Entity<ExerciseAddedObservation>(entity =>
        {
            entity.ToTable("ExerciseAddedObservations");
            entity.HasKey(e => new { e.WorkoutId, e.Timestamp });
            entity.OwnsOne(e => e.Metadata);
        });
    }
}
