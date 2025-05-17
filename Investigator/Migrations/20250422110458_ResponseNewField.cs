using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investigator.Migrations
{
    /// <inheritdoc />
    public partial class ResponseNewField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Filler",
                table: "Responses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_Filler",
                table: "Responses",
                column: "Filler");

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_AspNetUsers_Filler",
                table: "Responses",
                column: "Filler",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_AspNetUsers_Filler",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Responses_Filler",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "Filler",
                table: "Responses");
        }
    }
}
