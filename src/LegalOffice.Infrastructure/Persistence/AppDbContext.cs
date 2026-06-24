using LegalOffice.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Lawyer> Lawyers => Set<Lawyer>();
    public DbSet<LawyerSpecialty> LawyerSpecialties => Set<LawyerSpecialty>();
    public DbSet<LegalCase> Cases => Set<LegalCase>();
    public DbSet<CaseLawyer> CaseLawyers => Set<CaseLawyer>();
    public DbSet<CaseHearing> CaseHearings => Set<CaseHearing>();
    public DbSet<CaseDocument> CaseDocuments => Set<CaseDocument>();
    public DbSet<CaseTimeline> CaseTimelines => Set<CaseTimeline>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<LegalTask> LegalTasks => Set<LegalTask>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<CaseStageHistory> CaseStageHistories => Set<CaseStageHistory>();
    public DbSet<CaseAssignmentHistory> CaseAssignmentHistories => Set<CaseAssignmentHistory>();
    public DbSet<ConflictCheck> ConflictChecks => Set<ConflictCheck>();
    public DbSet<CaseInternalNote> CaseInternalNotes => Set<CaseInternalNote>();
    public DbSet<LegalConsultation> LegalConsultations => Set<LegalConsultation>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractVersion> ContractVersions => Set<ContractVersion>();
    public DbSet<PowerOfAttorney> PowerOfAttorneys => Set<PowerOfAttorney>();
    public DbSet<Judgment> Judgments => Set<Judgment>();
    public DbSet<ExecutionCase> ExecutionCases => Set<ExecutionCase>();
    public DbSet<Meeting> Meetings => Set<Meeting>();
    public DbSet<MeetingTask> MeetingTasks => Set<MeetingTask>();
    public DbSet<FeeAgreement> FeeAgreements => Set<FeeAgreement>();
    public DbSet<FeeInstallment> FeeInstallments => Set<FeeInstallment>();
    public DbSet<OfficeTreasury> OfficeTreasuries => Set<OfficeTreasury>();
    public DbSet<TreasuryTransaction> TreasuryTransactions => Set<TreasuryTransaction>();
    public DbSet<LookupType> LookupTypes => Set<LookupType>();
    public DbSet<Lookup> Lookups => Set<Lookup>();
    public DbSet<SystemPermission> SystemPermissions => Set<SystemPermission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<MessageTemplate> MessageTemplates => Set<MessageTemplate>();
    public DbSet<MessageLog> MessageLogs => Set<MessageLog>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Lawyer>().HasIndex(x => x.Email).IsUnique(false);
        builder.Entity<Client>().HasIndex(x => x.NationalId).IsUnique();
        builder.Entity<LegalCase>().HasIndex(x => new { x.CaseNumber, x.CaseTypeId, x.CaseYear }).IsUnique();

        builder.Entity<Lawyer>()
            .HasOne(x => x.User)
            .WithOne(x => x.Lawyer)
            .HasForeignKey<ApplicationUser>(x => x.LawyerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.UserType)
            .WithMany()
            .HasForeignKey(x => x.UserTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<LawyerSpecialty>()
            .HasIndex(x => new { x.LawyerId, x.CaseTypeId })
            .IsUnique();

        builder.Entity<Lookup>()
            .HasIndex(x => new { x.LookupTypeId, x.NameAr })
            .IsUnique();

        builder.Entity<LookupType>()
            .HasIndex(x => x.Code)
            .IsUnique();

        builder.Entity<LookupType>()
            .HasMany(x => x.Lookups)
            .WithOne(x => x.LookupType)
            .HasForeignKey(x => x.LookupTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<SystemPermission>()
            .HasIndex(x => x.Code)
            .IsUnique();

        builder.Entity<SystemPermission>()
            .HasIndex(x => new { x.Controller, x.Action })
            .IsUnique();

        builder.Entity<RolePermission>()
            .HasIndex(x => new { x.RoleName, x.SystemPermissionId })
            .IsUnique();

        builder.Entity<RolePermission>()
            .HasOne(x => x.SystemPermission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.SystemPermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CaseLawyer>()
            .HasIndex(x => new { x.CaseId, x.LawyerId })
            .IsUnique();

        builder.Entity<Lawyer>()
            .HasMany(x => x.Specialties)
            .WithOne(x => x.Lawyer)
            .HasForeignKey(x => x.LawyerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<LawyerSpecialty>()
            .HasOne(x => x.CaseType)
            .WithMany()
            .HasForeignKey(x => x.CaseTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LegalCase>()
            .HasOne(x => x.Client).WithMany(x => x.Cases).HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalCase>()
            .HasOne(x => x.CaseType).WithMany().HasForeignKey(x => x.CaseTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalCase>()
            .HasOne(x => x.CaseStatus).WithMany().HasForeignKey(x => x.CaseStatusId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalCase>()
            .HasOne(x => x.Court).WithMany().HasForeignKey(x => x.CourtId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalCase>()
            .HasOne(x => x.Priority).WithMany().HasForeignKey(x => x.PriorityId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalCase>()
            .HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<LegalCase>()
            .HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<LegalCase>()
            .HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<LegalCase>()
            .HasOne(x => x.WorkflowStageLookup).WithMany().HasForeignKey(x => x.WorkflowStageLookupId).OnDelete(DeleteBehavior.NoAction);

        builder.Entity<CaseLawyer>()
            .HasOne(x => x.Case).WithMany(x => x.CaseLawyers).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CaseLawyer>()
            .HasOne(x => x.Lawyer).WithMany(x => x.CaseLawyers).HasForeignKey(x => x.LawyerId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<CaseLawyer>()
            .HasOne(x => x.AccessLevel).WithMany().HasForeignKey(x => x.AccessLevelId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Lawyer>()
            .HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Lawyer>()
            .HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<CaseHearing>()
            .HasOne(x => x.Case).WithMany(x => x.Hearings).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CaseHearing>()
            .HasOne(x => x.HearingStatus).WithMany().HasForeignKey(x => x.HearingStatusId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CaseDocument>()
            .HasOne(x => x.Case).WithMany(x => x.Documents).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CaseDocument>()
            .HasOne(x => x.DocumentType).WithMany().HasForeignKey(x => x.DocumentTypeId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CaseTimeline>()
            .HasOne(x => x.Case).WithMany(x => x.Timelines).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Payment>()
            .HasOne(x => x.Case).WithMany(x => x.Payments).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Payment>()
            .HasOne(x => x.PaymentStatus).WithMany().HasForeignKey(x => x.PaymentStatusId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Payment>()
            .HasOne(x => x.PaymentMethod).WithMany().HasForeignKey(x => x.PaymentMethodId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Payment>()
            .HasOne(x => x.ReceivedByUser).WithMany().HasForeignKey(x => x.ReceivedByUserId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Payment>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Entity<Expense>().Property(x => x.Amount).HasColumnType("decimal(18,2)");
        builder.Entity<LegalCase>().Property(x => x.FeesAmount).HasColumnType("decimal(18,2)");
        builder.Entity<OfficeTreasury>().Property(x => x.CurrentBalance).HasColumnType("decimal(18,2)");
        builder.Entity<TreasuryTransaction>().Property(x => x.Amount).HasColumnType("decimal(18,2)");

        builder.Entity<Notification>()
            .HasIndex(x => new { x.LawyerId, x.IsRead, x.CreatedAt });

        builder.Entity<Notification>()
            .HasOne(x => x.Lawyer)
            .WithMany()
            .HasForeignKey(x => x.LawyerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Notification>()
            .HasOne(x => x.Case)
            .WithMany()
            .HasForeignKey(x => x.CaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Client>()
            .HasOne(x => x.Branch)
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Expense>()
            .HasOne(x => x.StatusLookup)
            .WithMany()
            .HasForeignKey(x => x.StatusLookupId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Expense>()
            .HasOne(x => x.SubmittedByUser)
            .WithMany()
            .HasForeignKey(x => x.SubmittedByUserId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.Entity<Expense>()
            .HasOne(x => x.ApprovedByUser)
            .WithMany()
            .HasForeignKey(x => x.ApprovedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Branch>()
            .HasOne(x => x.ManagerUser)
            .WithMany()
            .HasForeignKey(x => x.ManagerUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<LegalTask>()
            .HasOne(x => x.RelatedCase)
            .WithMany()
            .HasForeignKey(x => x.RelatedCaseId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Entity<LegalTask>()
            .HasOne(x => x.AssignedToUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalTask>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalTask>()
            .HasOne(x => x.PriorityLookup)
            .WithMany()
            .HasForeignKey(x => x.PriorityLookupId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalTask>()
            .HasOne(x => x.StatusLookup)
            .WithMany()
            .HasForeignKey(x => x.StatusLookupId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalTask>()
            .HasOne(x => x.TaskTypeLookup)
            .WithMany()
            .HasForeignKey(x => x.TaskTypeLookupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CaseStageHistory>()
            .HasOne(x => x.Case)
            .WithMany(x => x.StageHistory)
            .HasForeignKey(x => x.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CaseStageHistory>()
            .HasOne(x => x.FromStageLookup)
            .WithMany()
            .HasForeignKey(x => x.FromStageLookupId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Entity<CaseStageHistory>()
            .HasOne(x => x.ToStageLookup)
            .WithMany()
            .HasForeignKey(x => x.ToStageLookupId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<CaseStageHistory>()
            .HasOne(x => x.ChangedByUser)
            .WithMany()
            .HasForeignKey(x => x.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CaseAssignmentHistory>()
            .HasOne(x => x.Case)
            .WithMany()
            .HasForeignKey(x => x.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CaseAssignmentHistory>()
            .HasOne(x => x.Lawyer)
            .WithMany()
            .HasForeignKey(x => x.LawyerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<CaseAssignmentHistory>()
            .HasOne(x => x.ChangedByUser)
            .WithMany()
            .HasForeignKey(x => x.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ConflictCheck>()
            .HasOne(x => x.Case)
            .WithMany(x => x.ConflictChecks)
            .HasForeignKey(x => x.CaseId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Entity<ConflictCheck>()
            .HasOne(x => x.ResultStatusLookup)
            .WithMany()
            .HasForeignKey(x => x.ResultStatusLookupId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ConflictCheck>()
            .HasOne(x => x.CheckedByUser)
            .WithMany()
            .HasForeignKey(x => x.CheckedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CaseInternalNote>()
            .HasOne(x => x.Case)
            .WithMany(x => x.InternalNotes)
            .HasForeignKey(x => x.CaseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CaseInternalNote>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LegalConsultation>()
            .HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalConsultation>()
            .HasOne(x => x.AssignedLawyer).WithMany().HasForeignKey(x => x.AssignedLawyerId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalConsultation>()
            .HasOne(x => x.ConsultationTypeLookup).WithMany().HasForeignKey(x => x.ConsultationTypeLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalConsultation>()
            .HasOne(x => x.ConsultationStatusLookup).WithMany().HasForeignKey(x => x.ConsultationStatusLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LegalConsultation>()
            .HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<LegalConsultation>()
            .HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Contract>()
            .HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Contract>()
            .HasOne(x => x.AssignedLawyer).WithMany().HasForeignKey(x => x.AssignedLawyerId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Contract>()
            .HasOne(x => x.ContractTypeLookup).WithMany().HasForeignKey(x => x.ContractTypeLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Contract>()
            .HasOne(x => x.StatusLookup).WithMany().HasForeignKey(x => x.StatusLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Contract>()
            .HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Contract>()
            .HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<ContractVersion>()
            .HasOne(x => x.Contract).WithMany(x => x.Versions).HasForeignKey(x => x.ContractId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PowerOfAttorney>()
            .HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<PowerOfAttorney>()
            .HasOne(x => x.TypeLookup).WithMany().HasForeignKey(x => x.TypeLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<PowerOfAttorney>()
            .HasOne(x => x.StatusLookup).WithMany().HasForeignKey(x => x.StatusLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<PowerOfAttorney>()
            .HasOne(x => x.Case).WithMany().HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Judgment>()
            .HasOne(x => x.Case).WithMany().HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Judgment>()
            .HasOne(x => x.CourtLevelLookup).WithMany().HasForeignKey(x => x.CourtLevelLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<ExecutionCase>()
            .HasOne(x => x.Judgment).WithMany(x => x.ExecutionCases).HasForeignKey(x => x.JudgmentId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<ExecutionCase>()
            .HasOne(x => x.ExecutionStatusLookup).WithMany().HasForeignKey(x => x.ExecutionStatusLookupId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Meeting>()
            .HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Meeting>()
            .HasOne(x => x.Case).WithMany().HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Meeting>()
            .HasOne(x => x.AssignedUser).WithMany().HasForeignKey(x => x.AssignedUserId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Meeting>()
            .HasOne(x => x.StatusLookup).WithMany().HasForeignKey(x => x.StatusLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<MeetingTask>()
            .HasOne(x => x.Meeting).WithMany(x => x.MeetingTasks).HasForeignKey(x => x.MeetingId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<MeetingTask>()
            .HasOne(x => x.Task).WithMany().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FeeAgreement>()
            .HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<FeeAgreement>()
            .HasOne(x => x.Case).WithMany().HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.SetNull);
        builder.Entity<FeeAgreement>()
            .HasOne(x => x.FeeTypeLookup).WithMany().HasForeignKey(x => x.FeeTypeLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<FeeInstallment>()
            .HasOne(x => x.FeeAgreement).WithMany(x => x.Installments).HasForeignKey(x => x.FeeAgreementId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<FeeInstallment>()
            .HasOne(x => x.StatusLookup).WithMany().HasForeignKey(x => x.StatusLookupId).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TreasuryTransaction>()
            .HasOne(x => x.Treasury).WithMany().HasForeignKey(x => x.TreasuryId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<TreasuryTransaction>()
            .HasOne(x => x.TransactionTypeLookup).WithMany().HasForeignKey(x => x.TransactionTypeLookupId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<TreasuryTransaction>()
            .HasOne(x => x.Case).WithMany().HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.SetNull);
    }
}
