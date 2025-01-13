using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investigator.Migrations
{
    /// <inheritdoc />
    public partial class SalesForceField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalesForceUserId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesForceUserId",
                table: "AspNetUsers");
        }
    }
}
