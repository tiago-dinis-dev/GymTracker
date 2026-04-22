using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSetIndexToExerciseSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExerciseSets_ExercisePerformedId",
                table: "ExerciseSets");

            migrationBuilder.AddColumn<int>(
                name: "SetIndex",
                table: "ExerciseSets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSets_ExercisePerformedId_SetIndex",
                table: "ExerciseSets",
                columns: new[] { "ExercisePerformedId", "SetIndex" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExerciseSets_ExercisePerformedId_SetIndex",
                table: "ExerciseSets");

            migrationBuilder.DropColumn(
                name: "SetIndex",
                table: "ExerciseSets");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSets_ExercisePerformedId",
                table: "ExerciseSets",
                column: "ExercisePerformedId");
        }
    }
}
