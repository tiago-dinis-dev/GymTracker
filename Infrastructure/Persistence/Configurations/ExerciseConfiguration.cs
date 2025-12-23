using Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Name)
               .IsUnique();
        builder.Property(x => x.MuscleGroup).HasConversion<string>();
    }
}
