using Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("ExerciseId");
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MuscleGroup)
            .IsRequired()
            .HasMaxLength(50)
            .HasConversion<string>();

        builder.Property(x => x.Difficulty)
            .HasMaxLength(50)
            .HasColumnType("TEXT")
            .IsRequired(false);

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .HasColumnType("TEXT")
            .IsRequired(false);
    }
}
