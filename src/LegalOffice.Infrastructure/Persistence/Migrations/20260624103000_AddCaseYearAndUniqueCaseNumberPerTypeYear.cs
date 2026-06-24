using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalOffice.Infrastructure.Persistence.Migrations;

public partial class AddCaseYearAndUniqueCaseNumberPerTypeYear : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CaseYear",
            table: "Cases",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.Sql("UPDATE Cases SET CaseYear = YEAR(StartDate) WHERE CaseYear = 0");

        migrationBuilder.DropIndex(
            name: "IX_Cases_CaseNumber",
            table: "Cases");

        migrationBuilder.CreateIndex(
            name: "IX_Cases_CaseNumber_CaseTypeId_CaseYear",
            table: "Cases",
            columns: new[] { "CaseNumber", "CaseTypeId", "CaseYear" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Cases_CaseNumber_CaseTypeId_CaseYear",
            table: "Cases");

        migrationBuilder.AddColumn<int>(
            name: "CaseYear",
            table: "Cases",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_Cases_CaseNumber",
            table: "Cases",
            column: "CaseNumber",
            unique: true);
    }
}
