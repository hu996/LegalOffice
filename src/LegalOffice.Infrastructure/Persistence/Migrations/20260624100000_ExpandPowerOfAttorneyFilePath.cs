using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalOffice.Infrastructure.Persistence.Migrations;

public partial class ExpandPowerOfAttorneyFilePath : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "FilePath",
            table: "PowerOfAttorneys",
            type: "nvarchar(4000)",
            maxLength: 4000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "FilePath",
            table: "PowerOfAttorneys",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(4000)",
            oldMaxLength: 4000,
            oldNullable: true);
    }
}
