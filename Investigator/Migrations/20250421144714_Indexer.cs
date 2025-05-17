using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investigator.Migrations
{
    /// <inheritdoc />
    public partial class Indexer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FormFillers_Filler",
                table: "FormFillers");

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "Forms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormFillers_Filler_FormId",
                table: "FormFillers",
                columns: new[] { "Filler", "FormId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FormFillers_Filler_FormId",
                table: "FormFillers");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Forms");

            migrationBuilder.CreateIndex(
                name: "IX_FormFillers_Filler",
                table: "FormFillers",
                column: "Filler");
        }
    }
}
