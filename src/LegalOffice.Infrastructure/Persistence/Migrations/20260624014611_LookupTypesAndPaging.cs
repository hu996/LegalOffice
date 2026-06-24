using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LookupTypesAndPaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lookups_Type_NameAr",
                table: "Lookups");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Lookups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "Lookups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "Lookups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "LookupTypeId",
                table: "Lookups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LookupTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupTypes", x => x.Id);
                });

            migrationBuilder.Sql(@"
INSERT INTO LookupTypes (Code, NameAr, NameEn, IsActive, SortOrder)
SELECT DISTINCT l.[Type], l.[Type], l.[Type], CAST(1 AS bit), 0
FROM Lookups l
WHERE NOT EXISTS (
    SELECT 1
    FROM LookupTypes lt
    WHERE lt.Code = l.[Type]
)");

            migrationBuilder.Sql(@"
UPDATE l
SET l.LookupTypeId = lt.Id
FROM Lookups l
INNER JOIN LookupTypes lt ON lt.Code = l.[Type]
");

            migrationBuilder.CreateIndex(
                name: "IX_Lookups_LookupTypeId_NameAr",
                table: "Lookups",
                columns: new[] { "LookupTypeId", "NameAr" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupTypes_Code",
                table: "LookupTypes",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Lookups_LookupTypes_LookupTypeId",
                table: "Lookups",
                column: "LookupTypeId",
                principalTable: "LookupTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lookups_LookupTypes_LookupTypeId",
                table: "Lookups");

            migrationBuilder.DropTable(
                name: "LookupTypes");

            migrationBuilder.DropIndex(
                name: "IX_Lookups_LookupTypeId_NameAr",
                table: "Lookups");

            migrationBuilder.DropColumn(
                name: "LookupTypeId",
                table: "Lookups");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Lookups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NameEn",
                table: "Lookups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                table: "Lookups",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_Lookups_Type_NameAr",
                table: "Lookups",
                columns: new[] { "Type", "NameAr" },
                unique: true);
        }
    }
}
