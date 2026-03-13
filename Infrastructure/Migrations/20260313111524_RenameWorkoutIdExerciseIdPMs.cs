using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameWorkoutIdExerciseIdPMs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Workouts",
                newName: "WorkoutId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Exercises",
                newName: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkoutId",
                table: "Workouts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ExerciseId",
                table: "Exercises",
                newName: "Id");
        }
    }
}
