using Domain.Exercises;
using Domain.Users;
using Domain.Workouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.ToTable("Workouts");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).HasColumnName("WorkoutId");

        builder.Property(w => w.UserId).IsRequired();
        builder.HasOne<User>()
           .WithMany()
           .HasForeignKey(w => w.UserId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.Property(w => w.Status).HasConversion<string>();

        builder.OwnsMany(w => w.Exercises, eb =>
        {
            eb.ToTable("WorkoutExercises");

            eb.WithOwner().HasForeignKey("WorkoutId");

            eb.Property<Guid>("ExercisePerformedId").ValueGeneratedOnAdd();
            eb.HasKey("ExercisePerformedId");

            eb.Property(ep => ep.ExerciseId).IsRequired();

            eb.HasOne(typeof(Exercise))
                          .WithMany()
                          .HasForeignKey("ExerciseId")
                          .OnDelete(DeleteBehavior.Restrict);

            eb.HasIndex("WorkoutId", "ExerciseId").IsUnique();

            eb.OwnsMany(ep => ep.Sets, sb =>
            {
                sb.ToTable("ExerciseSets");

                sb.WithOwner().HasForeignKey("ExercisePerformedId");
                
                sb.Property<Guid>("SetRecordId").ValueGeneratedOnAdd();
                sb.HasKey("SetRecordId");
            });
        });

        builder.Navigation(w => w.Exercises).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
