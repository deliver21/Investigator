using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investigator.Migrations
{
    /// <inheritdoc />
    public partial class NullableUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JiraTickets_AspNetUsers_CreatedByUserId",
                table: "JiraTickets");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "JiraTickets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JiraTickets_ApplicationUserId",
                table: "JiraTickets",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JiraTickets_AspNetUsers_ApplicationUserId",
                table: "JiraTickets",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JiraTickets_AspNetUsers_CreatedByUserId",
                table: "JiraTickets",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JiraTickets_AspNetUsers_ApplicationUserId",
                table: "JiraTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_JiraTickets_AspNetUsers_CreatedByUserId",
                table: "JiraTickets");

            migrationBuilder.DropIndex(
                name: "IX_JiraTickets_ApplicationUserId",
                table: "JiraTickets");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "JiraTickets");

            migrationBuilder.AddForeignKey(
                name: "FK_JiraTickets_AspNetUsers_CreatedByUserId",
                table: "JiraTickets",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
