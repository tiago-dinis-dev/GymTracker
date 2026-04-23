using System;
using Microsoft.EntityFrameworkCore.Migrations;

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
                    WorkoutId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExerciseName = table.Column<string>(type: "TEXT", nullable: true),
                    MuscleGroup = table.Column<string>(type: "TEXT", nullable: true),
                    IntensityPercent1Rm = table.Column<decimal>(type: "TEXT", nullable: true),
                    VolumeData = table.Column<decimal>(type: "TEXT", nullable: false),
                    OrderInWorkout = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseAddedObservations", x => new { x.WorkoutId, x.Timestamp });
                });

            migrationBuilder.CreateTable(
                name: "WorkoutCompletedObservations",
                columns: table => new
                {
                    WorkoutId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalVolume = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalDuration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    ExerciseCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutCompletedObservations", x => new { x.WorkoutId, x.Timestamp });
                });

            migrationBuilder.CreateTable(
                name: "ExerciseAddedObservationSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<float>(type: "REAL", nullable: false),
                    Estimated1Rm = table.Column<decimal>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WorkoutId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseAddedObservationSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseAddedObservationSets_ExerciseAddedObservations_WorkoutId_Timestamp",
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
                name: "WorkoutCompletedObservations");

            migrationBuilder.DropTable(
                name: "ExerciseAddedObservations");
        }
    }
}
