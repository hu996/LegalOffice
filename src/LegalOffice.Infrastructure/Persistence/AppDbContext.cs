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
        builder.Entity<LegalCase>().HasIndex(x => x.CaseNumber).IsUnique();

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

        builder.Entity<CaseLawyer>()
            .HasOne(x => x.Case).WithMany(x => x.CaseLawyers).HasForeignKey(x => x.CaseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<CaseLawyer>()
            .HasOne(x => x.Lawyer).WithMany(x => x.CaseLawyers).HasForeignKey(x => x.LawyerId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<CaseLawyer>()
            .HasOne(x => x.AccessLevel).WithMany().HasForeignKey(x => x.AccessLevelId).OnDelete(DeleteBehavior.Restrict);

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
    }
}
