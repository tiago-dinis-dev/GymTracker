using Domain.Workouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Status).HasConversion<string>();
        builder.Navigation("_exercises").UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany<ExercisePerformed>("_exercises", eb =>
        {
            eb.WithOwner().HasForeignKey("WorkoutId");

            eb.OwnsMany(e => e.Sets);
        });
    }
}
