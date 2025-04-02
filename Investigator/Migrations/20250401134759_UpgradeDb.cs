using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Investigator.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionDate",
                table: "Forms");

            migrationBuilder.AddColumn<int>(
                name: "FormFillerId",
                table: "Responses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormFillers",
                columns: table => new
                {
                    FormFillerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filler = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormFillers", x => x.FormFillerId);
                    table.ForeignKey(
                        name: "FK_FormFillers_AspNetUsers_Filler",
                        column: x => x.Filler,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FormFillers_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Responses_FormFillerId",
                table: "Responses",
                column: "FormFillerId");

            migrationBuilder.CreateIndex(
                name: "IX_FormFillers_Filler",
                table: "FormFillers",
                column: "Filler");

            migrationBuilder.CreateIndex(
                name: "IX_FormFillers_FormId",
                table: "FormFillers",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_FormFillers_FormFillerId",
                table: "Responses",
                column: "FormFillerId",
                principalTable: "FormFillers",
                principalColumn: "FormFillerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_FormFillers_FormFillerId",
                table: "Responses");

            migrationBuilder.DropTable(
                name: "FormFillers");

            migrationBuilder.DropIndex(
                name: "IX_Responses_FormFillerId",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "FormFillerId",
                table: "Responses");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmissionDate",
                table: "Forms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
