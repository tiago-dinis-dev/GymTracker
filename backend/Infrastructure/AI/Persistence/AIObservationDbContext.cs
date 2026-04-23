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

            entity.OwnsOne(e => e.Metadata, md =>
            {
                md.Property(m => m.Name).HasColumnName("ExerciseName");
                md.Property(m => m.MuscleGroup).HasConversion<string>().HasColumnName("MuscleGroup");
                md.Property(m => m.IntensityPercent1Rm).HasColumnName("IntensityPercent1Rm");

                md.OwnsMany(m => m.Sets, sb =>
                {
                    sb.ToTable("ExerciseAddedObservationSets");
                    sb.WithOwner().HasForeignKey("WorkoutId", "Timestamp");
                    sb.Property<int>("Id").ValueGeneratedOnAdd();
                    sb.HasKey("Id");

                    sb.Property(s => s.Reps).IsRequired();
                    sb.Property(s => s.Weight).IsRequired();
                    sb.Property(s => s.Estimated1Rm).IsRequired();
                });
            });
        });
    }
}
