using Microsoft.AspNetCore.Identity;

namespace LegalOffice.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool MustChangePassword { get; set; }
    public int? UserTypeId { get; set; }
    public Lookup? UserType { get; set; }
    public int? DepartmentId { get; set; }
    public Lookup? Department { get; set; }
    public int? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public int? LawyerId { get; set; }
    public Lawyer? Lawyer { get; set; }
}
