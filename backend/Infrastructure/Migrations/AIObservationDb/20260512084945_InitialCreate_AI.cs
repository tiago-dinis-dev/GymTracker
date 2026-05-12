using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations.AIObservationDb
{
    /// <inheritdoc />
    public partial class InitialCreate_AI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExerciseAddedObservations",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "text", nullable: true),
                    MuscleGroup = table.Column<string>(type: "text", nullable: true),
                    IntensityPercent1Rm = table.Column<decimal>(type: "numeric", nullable: true),
                    VolumeData = table.Column<decimal>(type: "numeric", nullable: false),
                    OrderInWorkout = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseAddedObservations", x => new { x.WorkoutId, x.Timestamp });
                });

            migrationBuilder.CreateTable(
                name: "ExerciseStats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseName = table.Column<string>(type: "text", nullable: false),
                    MuscleGroup = table.Column<string>(type: "text", nullable: false),
                    TotalSets = table.Column<int>(type: "integer", nullable: false),
                    TotalReps = table.Column<int>(type: "integer", nullable: false),
                    TotalVolume = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageWeight = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseStats", x => new { x.UserId, x.ExerciseName });
                });

            migrationBuilder.CreateTable(
                name: "MuscleGroupStats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MuscleGroup = table.Column<string>(type: "text", nullable: false),
                    TotalExercises = table.Column<int>(type: "integer", nullable: false),
                    TotalVolume = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalSets = table.Column<int>(type: "integer", nullable: false),
                    TotalReps = table.Column<int>(type: "integer", nullable: false),
                    AverageIntensity = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuscleGroupStats", x => new { x.UserId, x.MuscleGroup });
                });

            migrationBuilder.CreateTable(
                name: "WorkoutCompletedObservations",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalVolume = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalDuration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ExerciseCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutCompletedObservations", x => new { x.WorkoutId, x.Timestamp });
                });

            migrationBuilder.CreateTable(
                name: "ExerciseAddedObservationSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Estimated1Rm = table.Column<decimal>(type: "numeric", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseAddedObservationSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseAddedObservationSets_ExerciseAddedObservations_Work~",
                        columns: x => new { x.WorkoutId, x.Timestamp },
                        principalTable: "ExerciseAddedObservations",
                        principalColumns: new[] { "WorkoutId", "Timestamp" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseAddedObservationSets_WorkoutId_Timestamp",
                table: "ExerciseAddedObservationSets",
                columns: new[] { "WorkoutId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseAddedObservationSets");

            migrationBuilder.DropTable(
                name: "ExerciseStats");

            migrationBuilder.DropTable(
                name: "MuscleGroupStats");

            migrationBuilder.DropTable(
                name: "WorkoutCompletedObservations");

            migrationBuilder.DropTable(
                name: "ExerciseAddedObservations");
        }
    }
}
