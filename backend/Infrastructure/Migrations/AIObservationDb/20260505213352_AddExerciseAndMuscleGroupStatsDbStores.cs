using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.AIObservationDb
{
    /// <inheritdoc />
    public partial class AddExerciseAndMuscleGroupStatsDbStores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExerciseStats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExerciseName = table.Column<string>(type: "TEXT", nullable: false),
                    MuscleGroup = table.Column<string>(type: "TEXT", nullable: false),
                    TotalSets = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalReps = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalVolume = table.Column<decimal>(type: "TEXT", nullable: false),
                    AverageWeight = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseStats", x => new { x.UserId, x.ExerciseName });
                });

            migrationBuilder.CreateTable(
                name: "MuscleGroupStats",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MuscleGroup = table.Column<string>(type: "TEXT", nullable: false),
                    TotalExercises = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalVolume = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalSets = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalReps = table.Column<int>(type: "INTEGER", nullable: false),
                    AverageIntensity = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuscleGroupStats", x => new { x.UserId, x.MuscleGroup });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseStats");

            migrationBuilder.DropTable(
                name: "MuscleGroupStats");
        }
    }
}
