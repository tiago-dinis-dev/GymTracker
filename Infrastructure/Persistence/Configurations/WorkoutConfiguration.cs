using Domain.Workouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.UserId).IsRequired();
        builder.Property(w => w.Status).HasConversion<string>();

        builder.OwnsMany(w => w.Exercises, eb =>
        {
            eb.WithOwner().HasForeignKey("WorkoutId");
            eb.OwnsMany(e => e.Sets);
        });
        builder.Navigation(w => w.Exercises).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
