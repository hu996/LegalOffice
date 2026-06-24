using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalOffice.Infrastructure.Persistence.Migrations;

public partial class AddCaseCreatedByUserId : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CreatedByUserId",
            table: "Cases",
            type: "nvarchar(450)",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Cases_CreatedByUserId",
            table: "Cases",
            column: "CreatedByUserId");

        migrationBuilder.AddForeignKey(
            name: "FK_Cases_AspNetUsers_CreatedByUserId",
            table: "Cases",
            column: "CreatedByUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Cases_AspNetUsers_CreatedByUserId",
            table: "Cases");

        migrationBuilder.DropIndex(
            name: "IX_Cases_CreatedByUserId",
            table: "Cases");

        migrationBuilder.DropColumn(
            name: "CreatedByUserId",
            table: "Cases");
    }
}
