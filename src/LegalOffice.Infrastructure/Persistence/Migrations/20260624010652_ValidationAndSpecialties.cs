using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ValidationAndSpecialties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessLevel",
                table: "CaseLawyers",
                newName: "AccessLevelId");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.Sql(@"
UPDATE Clients
SET NationalId = CAST(10000000000000 + Id AS nvarchar(14))
WHERE NationalId = '';
");

            migrationBuilder.AlterColumn<string>(
                name: "CaseNumber",
                table: "Cases",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "PriorityId",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "CaseDocuments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LawyerSpecialties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LawyerId = table.Column<int>(type: "int", nullable: false),
                    CaseTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LawyerSpecialties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LawyerSpecialties_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LawyerSpecialties_Lookups_CaseTypeId",
                        column: x => x.CaseTypeId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'View')
    INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CaseAccessLevel', N'عرض فقط', 'View', 1);
IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'Edit')
    INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CaseAccessLevel', N'تعديل', 'Edit', 1);
IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'Manage')
    INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CaseAccessLevel', N'إدارة كاملة', 'Manage', 1);
IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'Low')
    INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CasePriority', N'منخفضة', 'Low', 1);
IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'Medium')
    INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CasePriority', N'متوسطة', 'Medium', 1);
IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'High')
    INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CasePriority', N'عالية', 'High', 1);
IF NOT EXISTS (SELECT 1 FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'Critical')
    INSERT INTO Lookups (Type, NameAr, NameEn, IsActive) VALUES ('CasePriority', N'حرجة', 'Critical', 1);

UPDATE cl
SET cl.AccessLevelId =
    CASE cl.AccessLevelId
        WHEN 1 THEN (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'View' ORDER BY Id)
        WHEN 2 THEN (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'Edit' ORDER BY Id)
        WHEN 3 THEN (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'Manage' ORDER BY Id)
        ELSE (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CaseAccessLevel' AND NameEn = 'View' ORDER BY Id)
    END
FROM CaseLawyers cl;

UPDATE c
SET c.PriorityId = (SELECT TOP 1 Id FROM Lookups WHERE Type = 'CasePriority' AND NameEn = 'Medium' ORDER BY Id)
FROM Cases c;
");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_NationalId",
                table: "Clients",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cases_CaseNumber",
                table: "Cases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cases_PriorityId",
                table: "Cases",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseLawyers_AccessLevelId",
                table: "CaseLawyers",
                column: "AccessLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_LawyerSpecialties_CaseTypeId",
                table: "LawyerSpecialties",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LawyerSpecialties_LawyerId_CaseTypeId",
                table: "LawyerSpecialties",
                columns: new[] { "LawyerId", "CaseTypeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseLawyers_Lookups_AccessLevelId",
                table: "CaseLawyers",
                column: "AccessLevelId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Lookups_PriorityId",
                table: "Cases",
                column: "PriorityId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Cases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseLawyers_Lookups_AccessLevelId",
                table: "CaseLawyers");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Lookups_PriorityId",
                table: "Cases");

            migrationBuilder.DropTable(
                name: "LawyerSpecialties");

            migrationBuilder.DropIndex(
                name: "IX_Clients_NationalId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Cases_CaseNumber",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Cases_PriorityId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_CaseLawyers_AccessLevelId",
                table: "CaseLawyers");

            migrationBuilder.DropColumn(
                name: "PriorityId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "CaseDocuments");

            migrationBuilder.RenameColumn(
                name: "AccessLevelId",
                table: "CaseLawyers",
                newName: "AccessLevel");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CaseNumber",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
