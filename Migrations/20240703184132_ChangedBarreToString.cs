using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CagedApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangedBarreToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chords_Progressions_ProgressionId",
                table: "Chords");

            migrationBuilder.AlterColumn<string>(
                name: "Barre",
                table: "Chords",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chords_Progressions_ProgressionId",
                table: "Chords",
                column: "ProgressionId",
                principalTable: "Progressions",
                principalColumn: "ProgressionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chords_Progressions_ProgressionId",
                table: "Chords");

            migrationBuilder.AlterColumn<int>(
                name: "Barre",
                table: "Chords",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chords_Progressions_ProgressionId",
                table: "Chords",
                column: "ProgressionId",
                principalTable: "Progressions",
                principalColumn: "ProgressionId");
        }
    }
}
