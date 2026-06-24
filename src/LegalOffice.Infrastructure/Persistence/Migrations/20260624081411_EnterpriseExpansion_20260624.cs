using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegalOffice.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseExpansion_20260624 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Lawyers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                table: "Lawyers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Lawyers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Lawyers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Expenses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByUserId",
                table: "Expenses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusLookupId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmittedByUserId",
                table: "Expenses",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Clients",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Clients",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OpponentName",
                table: "Cases",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OpponentLawyer",
                table: "Cases",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Cases",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Circuit",
                table: "Cases",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Cases",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Cases",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastStageChangedAt",
                table: "Cases",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowStageLookupId",
                table: "Cases",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "CaseHearings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NextRequirements",
                table: "CaseHearings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourtDecision",
                table: "CaseHearings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ManagerUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Branches_AspNetUsers_ManagerUserId",
                        column: x => x.ManagerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CaseAssignmentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    LawyerId = table.Column<int>(type: "int", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseAssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseAssignmentHistories_AspNetUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseAssignmentHistories_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseAssignmentHistories_Lawyers_LawyerId",
                        column: x => x.LawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseInternalNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseInternalNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseInternalNotes_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseInternalNotes_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseStageHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    FromStageLookupId = table.Column<int>(type: "int", nullable: true),
                    ToStageLookupId = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseStageHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseStageHistories_AspNetUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseStageHistories_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseStageHistories_Lookups_FromStageLookupId",
                        column: x => x.FromStageLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CaseStageHistories_Lookups_ToStageLookupId",
                        column: x => x.ToStageLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConflictChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpponentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaseId = table.Column<int>(type: "int", nullable: true),
                    ResultStatusLookupId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CheckedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConflictChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConflictChecks_AspNetUsers_CheckedByUserId",
                        column: x => x.CheckedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConflictChecks_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ConflictChecks_Lookups_ResultStatusLookupId",
                        column: x => x.ResultStatusLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeeAgreements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    CaseId = table.Column<int>(type: "int", nullable: true),
                    FeeTypeLookupId = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeAgreements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeAgreements_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FeeAgreements_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeAgreements_Lookups_FeeTypeLookupId",
                        column: x => x.FeeTypeLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Judgments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseId = table.Column<int>(type: "int", nullable: false),
                    JudgmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourtLevelLookupId = table.Column<int>(type: "int", nullable: false),
                    JudgmentSummary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    JudgmentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Judgments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Judgments_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Judgments_Lookups_CourtLevelLookupId",
                        column: x => x.CourtLevelLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LegalTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RelatedCaseId = table.Column<int>(type: "int", nullable: true),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PriorityLookupId = table.Column<int>(type: "int", nullable: false),
                    StatusLookupId = table.Column<int>(type: "int", nullable: false),
                    TaskTypeLookupId = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalTasks_AspNetUsers_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalTasks_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalTasks_Cases_RelatedCaseId",
                        column: x => x.RelatedCaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LegalTasks_Lookups_PriorityLookupId",
                        column: x => x.PriorityLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalTasks_Lookups_StatusLookupId",
                        column: x => x.StatusLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalTasks_Lookups_TaskTypeLookupId",
                        column: x => x.TaskTypeLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    CaseId = table.Column<int>(type: "int", nullable: true),
                    AssignedUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MeetingResult = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StatusLookupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meetings_AspNetUsers_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Meetings_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Meetings_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Meetings_Lookups_StatusLookupId",
                        column: x => x.StatusLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfficeTreasuries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeTreasuries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PowerOfAttorneys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PowerNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    TypeLookupId = table.Column<int>(type: "int", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationOffice = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StatusLookupId = table.Column<int>(type: "int", nullable: false),
                    CaseId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PowerOfAttorneys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PowerOfAttorneys_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PowerOfAttorneys_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerOfAttorneys_Lookups_StatusLookupId",
                        column: x => x.StatusLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PowerOfAttorneys_Lookups_TypeLookupId",
                        column: x => x.TypeLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContractTypeLookupId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    AssignedLawyerId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusLookupId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Lawyers_AssignedLawyerId",
                        column: x => x.AssignedLawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Lookups_ContractTypeLookupId",
                        column: x => x.ContractTypeLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contracts_Lookups_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Contracts_Lookups_StatusLookupId",
                        column: x => x.StatusLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LegalConsultations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsultationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    AssignedLawyerId = table.Column<int>(type: "int", nullable: false),
                    ConsultationTypeLookupId = table.Column<int>(type: "int", nullable: false),
                    ConsultationStatusLookupId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsultationFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LegalOpinion = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalConsultations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LegalConsultations_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LegalConsultations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalConsultations_Lawyers_AssignedLawyerId",
                        column: x => x.AssignedLawyerId,
                        principalTable: "Lawyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalConsultations_Lookups_ConsultationStatusLookupId",
                        column: x => x.ConsultationStatusLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalConsultations_Lookups_ConsultationTypeLookupId",
                        column: x => x.ConsultationTypeLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LegalConsultations_Lookups_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FeeInstallments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeeAgreementId = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StatusLookupId = table.Column<int>(type: "int", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeInstallments_FeeAgreements_FeeAgreementId",
                        column: x => x.FeeAgreementId,
                        principalTable: "FeeAgreements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeeInstallments_Lookups_StatusLookupId",
                        column: x => x.StatusLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExecutionCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JudgmentId = table.Column<int>(type: "int", nullable: false),
                    ExecutionStatusLookupId = table.Column<int>(type: "int", nullable: false),
                    ExecutionOfficer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExecutionNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExecutionCases_Judgments_JudgmentId",
                        column: x => x.JudgmentId,
                        principalTable: "Judgments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExecutionCases_Lookups_ExecutionStatusLookupId",
                        column: x => x.ExecutionStatusLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeetingTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeetingTasks_LegalTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "LegalTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingTasks_Meetings_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TreasuryTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TreasuryId = table.Column<int>(type: "int", nullable: false),
                    TransactionTypeLookupId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CaseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreasuryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreasuryTransactions_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TreasuryTransactions_Lookups_TransactionTypeLookupId",
                        column: x => x.TransactionTypeLookupId,
                        principalTable: "Lookups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreasuryTransactions_OfficeTreasuries_TreasuryId",
                        column: x => x.TreasuryId,
                        principalTable: "OfficeTreasuries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractVersions_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lawyers_BranchId",
                table: "Lawyers",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Lawyers_DepartmentId",
                table: "Lawyers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ApprovedByUserId",
                table: "Expenses",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_StatusLookupId",
                table: "Expenses",
                column: "StatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_SubmittedByUserId",
                table: "Expenses",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_BranchId",
                table: "Clients",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_BranchId",
                table: "Cases",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_DepartmentId",
                table: "Cases",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_WorkflowStageLookupId",
                table: "Cases",
                column: "WorkflowStageLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BranchId",
                table: "AspNetUsers",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_ManagerUserId",
                table: "Branches",
                column: "ManagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseAssignmentHistories_CaseId",
                table: "CaseAssignmentHistories",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseAssignmentHistories_ChangedByUserId",
                table: "CaseAssignmentHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseAssignmentHistories_LawyerId",
                table: "CaseAssignmentHistories",
                column: "LawyerId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseInternalNotes_CaseId",
                table: "CaseInternalNotes",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseInternalNotes_CreatedByUserId",
                table: "CaseInternalNotes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseStageHistories_CaseId",
                table: "CaseStageHistories",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseStageHistories_ChangedByUserId",
                table: "CaseStageHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseStageHistories_FromStageLookupId",
                table: "CaseStageHistories",
                column: "FromStageLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseStageHistories_ToStageLookupId",
                table: "CaseStageHistories",
                column: "ToStageLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictChecks_CaseId",
                table: "ConflictChecks",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictChecks_CheckedByUserId",
                table: "ConflictChecks",
                column: "CheckedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConflictChecks_ResultStatusLookupId",
                table: "ConflictChecks",
                column: "ResultStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_AssignedLawyerId",
                table: "Contracts",
                column: "AssignedLawyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_BranchId",
                table: "Contracts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId",
                table: "Contracts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ContractTypeLookupId",
                table: "Contracts",
                column: "ContractTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_DepartmentId",
                table: "Contracts",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_StatusLookupId",
                table: "Contracts",
                column: "StatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractVersions_ContractId",
                table: "ContractVersions",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCases_ExecutionStatusLookupId",
                table: "ExecutionCases",
                column: "ExecutionStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionCases_JudgmentId",
                table: "ExecutionCases",
                column: "JudgmentId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeAgreements_CaseId",
                table: "FeeAgreements",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeAgreements_ClientId",
                table: "FeeAgreements",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeAgreements_FeeTypeLookupId",
                table: "FeeAgreements",
                column: "FeeTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeInstallments_FeeAgreementId",
                table: "FeeInstallments",
                column: "FeeAgreementId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeInstallments_StatusLookupId",
                table: "FeeInstallments",
                column: "StatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_Judgments_CaseId",
                table: "Judgments",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Judgments_CourtLevelLookupId",
                table: "Judgments",
                column: "CourtLevelLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalConsultations_AssignedLawyerId",
                table: "LegalConsultations",
                column: "AssignedLawyerId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalConsultations_BranchId",
                table: "LegalConsultations",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalConsultations_ClientId",
                table: "LegalConsultations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalConsultations_ConsultationStatusLookupId",
                table: "LegalConsultations",
                column: "ConsultationStatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalConsultations_ConsultationTypeLookupId",
                table: "LegalConsultations",
                column: "ConsultationTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalConsultations_DepartmentId",
                table: "LegalConsultations",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalTasks_AssignedToUserId",
                table: "LegalTasks",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalTasks_CreatedByUserId",
                table: "LegalTasks",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalTasks_PriorityLookupId",
                table: "LegalTasks",
                column: "PriorityLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalTasks_RelatedCaseId",
                table: "LegalTasks",
                column: "RelatedCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalTasks_StatusLookupId",
                table: "LegalTasks",
                column: "StatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_LegalTasks_TaskTypeLookupId",
                table: "LegalTasks",
                column: "TaskTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_AssignedUserId",
                table: "Meetings",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_CaseId",
                table: "Meetings",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_ClientId",
                table: "Meetings",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_StatusLookupId",
                table: "Meetings",
                column: "StatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingTasks_MeetingId",
                table: "MeetingTasks",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_MeetingTasks_TaskId",
                table: "MeetingTasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerOfAttorneys_CaseId",
                table: "PowerOfAttorneys",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerOfAttorneys_ClientId",
                table: "PowerOfAttorneys",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerOfAttorneys_StatusLookupId",
                table: "PowerOfAttorneys",
                column: "StatusLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_PowerOfAttorneys_TypeLookupId",
                table: "PowerOfAttorneys",
                column: "TypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_CaseId",
                table: "TreasuryTransactions",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_TransactionTypeLookupId",
                table: "TreasuryTransactions",
                column: "TransactionTypeLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_TreasuryTransactions_TreasuryId",
                table: "TreasuryTransactions",
                column: "TreasuryId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Branches_BranchId",
                table: "AspNetUsers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Branches_BranchId",
                table: "Cases",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Lookups_DepartmentId",
                table: "Cases",
                column: "DepartmentId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Lookups_WorkflowStageLookupId",
                table: "Cases",
                column: "WorkflowStageLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Branches_BranchId",
                table: "Clients",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_ApprovedByUserId",
                table: "Expenses",
                column: "ApprovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_SubmittedByUserId",
                table: "Expenses",
                column: "SubmittedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Lookups_StatusLookupId",
                table: "Expenses",
                column: "StatusLookupId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Lawyers_Branches_BranchId",
                table: "Lawyers",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Lawyers_Lookups_DepartmentId",
                table: "Lawyers",
                column: "DepartmentId",
                principalTable: "Lookups",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Branches_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Branches_BranchId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Lookups_DepartmentId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Lookups_WorkflowStageLookupId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Branches_BranchId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_ApprovedByUserId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_SubmittedByUserId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Lookups_StatusLookupId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Lawyers_Branches_BranchId",
                table: "Lawyers");

            migrationBuilder.DropForeignKey(
                name: "FK_Lawyers_Lookups_DepartmentId",
                table: "Lawyers");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CaseAssignmentHistories");

            migrationBuilder.DropTable(
                name: "CaseInternalNotes");

            migrationBuilder.DropTable(
                name: "CaseStageHistories");

            migrationBuilder.DropTable(
                name: "ConflictChecks");

            migrationBuilder.DropTable(
                name: "ContractVersions");

            migrationBuilder.DropTable(
                name: "ExecutionCases");

            migrationBuilder.DropTable(
                name: "FeeInstallments");

            migrationBuilder.DropTable(
                name: "LegalConsultations");

            migrationBuilder.DropTable(
                name: "MeetingTasks");

            migrationBuilder.DropTable(
                name: "PowerOfAttorneys");

            migrationBuilder.DropTable(
                name: "TreasuryTransactions");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Judgments");

            migrationBuilder.DropTable(
                name: "FeeAgreements");

            migrationBuilder.DropTable(
                name: "LegalTasks");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "OfficeTreasuries");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropIndex(
                name: "IX_Lawyers_BranchId",
                table: "Lawyers");

            migrationBuilder.DropIndex(
                name: "IX_Lawyers_DepartmentId",
                table: "Lawyers");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ApprovedByUserId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_StatusLookupId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_SubmittedByUserId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Clients_BranchId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Cases_BranchId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Cases_DepartmentId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_Cases_WorkflowStageLookupId",
                table: "Cases");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_BranchId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Lawyers");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ApprovedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "StatusLookupId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "SubmittedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "LastStageChangedAt",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "WorkflowStageLookupId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Lawyers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobTitle",
                table: "Lawyers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OpponentName",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OpponentLawyer",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Circuit",
                table: "Cases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "CaseHearings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NextRequirements",
                table: "CaseHearings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourtDecision",
                table: "CaseHearings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
