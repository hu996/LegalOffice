using LegalOffice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LegalOffice.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { "Admin", "Manager", "Lawyer", "Secretary" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (!await roleManager.RoleExistsAsync("HR"))
        {
            await roleManager.CreateAsync(new IdentityRole("HR"));
        }

        var adminEmail = "admin@legal.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "مدير النظام",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, "123456");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        await EnsureLookupTypeAsync(db, "CaseType", "أنواع القضايا", "Case Types", 10);
        await EnsureLookupTypeAsync(db, "CaseStatus", "حالات القضايا", "Case Statuses", 20);
        await EnsureLookupTypeAsync(db, "HearingStatus", "حالات الجلسات", "Hearing Statuses", 30);
        await EnsureLookupTypeAsync(db, "Court", "المحاكم", "Courts", 40);
        await EnsureLookupTypeAsync(db, "DocumentType", "أنواع المستندات", "Document Types", 50);
        await EnsureLookupTypeAsync(db, "ExpenseType", "أنواع المصروفات", "Expense Types", 60);
        await EnsureLookupTypeAsync(db, "PaymentStatus", "حالات الدفعات", "Payment Statuses", 70);
        await EnsureLookupTypeAsync(db, "PaymentMethod", "طرق الدفع", "Payment Methods", 80);
        await EnsureLookupTypeAsync(db, "CasePriority", "أولويات القضايا", "Case Priorities", 90);
        await EnsureLookupTypeAsync(db, "CaseAccessLevel", "صلاحيات المحامي على القضية", "Case Access Levels", 100);
        await EnsureLookupTypeAsync(db, "UserType", "أنواع المستخدمين", "User Types", 110);
        await EnsureLookupTypeAsync(db, "Department", "الإدارات", "Departments", 120);

        await EnsureLookupGroupAsync(db, "CaseType", new[]
        {
            ("مدني", "Civil"),
            ("جنائي", "Criminal"),
            ("أسرة", "Family"),
            ("تجاري", "Commercial"),
            ("عمال", "Labor")
        });

        await EnsureLookupGroupAsync(db, "CaseStatus", new[]
        {
            ("جديدة", "New"),
            ("قيد المتابعة", "InProgress"),
            ("مؤجلة", "Deferred"),
            ("محجوزة للحكم", "Reserved"),
            ("تم الحكم", "Judged"),
            ("مغلقة", "Closed")
        });

        await EnsureLookupGroupAsync(db, "HearingStatus", new[]
        {
            ("قادمة", "Upcoming"),
            ("تمت", "Done"),
            ("مؤجلة", "Deferred"),
            ("ملغية", "Cancelled"),
            ("صدر الحكم", "Judged")
        });

        await EnsureLookupGroupAsync(db, "Court", new[]
        {
            ("محكمة جنوب القاهرة", "SouthCairo"),
            ("محكمة شمال القاهرة", "NorthCairo"),
            ("محكمة الأسرة", "FamilyCourt")
        });

        await EnsureLookupGroupAsync(db, "DocumentType", new[]
        {
            ("توكيـل", "PowerOfAttorney"),
            ("مذكرة", "Memo"),
            ("حكم", "Judgment"),
            ("عقد", "Contract"),
            ("إيصال", "Receipt")
        });

        await EnsureLookupGroupAsync(db, "ExpenseType", new[]
        {
            ("رسوم محكمة", "CourtFees"),
            ("انتقال", "Transport"),
            ("إدارية", "Admin")
        });

        await EnsureLookupGroupAsync(db, "PaymentStatus", new[]
        {
            ("مسجل", "Recorded"),
            ("مستلم", "Received"),
            ("معلق", "Pending"),
            ("ملغي", "Cancelled")
        });

        await EnsureLookupGroupAsync(db, "PaymentMethod", new[]
        {
            ("نقدي", "Cash"),
            ("تحويل بنكي", "BankTransfer"),
            ("شيك", "Cheque"),
            ("إنستاباي", "Instapay")
        });

        await EnsureLookupGroupAsync(db, "CasePriority", new[]
        {
            ("منخفضة", "Low"),
            ("متوسطة", "Medium"),
            ("عالية", "High"),
            ("حرجة", "Critical")
        });

        await EnsureLookupGroupAsync(db, "CaseAccessLevel", new[]
        {
            ("عرض فقط", "View"),
            ("تعديل", "Edit"),
            ("إدارة كاملة", "Manage")
        });

        await EnsureLookupGroupAsync(db, "UserType", new[]
        {
            ("محامي", "Lawyer"),
            ("سكرتارية", "Secretary"),
            ("موارد بشرية", "HR"),
            ("أدمن", "Admin"),
            ("مدير", "Manager")
        });

        await EnsureLookupGroupAsync(db, "Department", new[]
        {
            ("الشؤون القانونية", "Legal"),
            ("الموارد البشرية", "HR"),
            ("السكرتارية", "Secretariat"),
            ("المالية", "Finance"),
            ("الإدارة", "Administration")
        });

        await EnsurePermissionAsync(db, "Dashboard.View", "لوحة التحكم", "Dashboard", "Index", "الرئيسية", 10, true);
        await EnsurePermissionAsync(db, "Dashboard.ViewAll", "عرض شامل للوحة التحكم", "Dashboard", "ViewAll", "الرئيسية", 11, false);
        await EnsurePermissionAsync(db, "Cases.View", "القضايا", "Cases", "Index", "العمل", 20, true);
        await EnsurePermissionAsync(db, "Cases.Create", "إضافة قضية", "Cases", "Create", "العمل", 21, false);
        await EnsurePermissionAsync(db, "Cases.Edit", "تعديل قضية", "Cases", "Edit", "العمل", 22, false);
        await EnsurePermissionAsync(db, "Cases.Details", "تفاصيل القضية", "Cases", "Details", "العمل", 23, false);
        await EnsurePermissionAsync(db, "Cases.ChangeStatus", "تغيير حالة القضية", "Cases", "ChangeStatus", "العمل", 24, false);
        await EnsurePermissionAsync(db, "Clients.View", "العملاء", "Clients", "Index", "العملاء", 30, true);
        await EnsurePermissionAsync(db, "Clients.Create", "إضافة عميل", "Clients", "Create", "العملاء", 31, false);
        await EnsurePermissionAsync(db, "Clients.Edit", "تعديل عميل", "Clients", "Edit", "العملاء", 32, false);
        await EnsurePermissionAsync(db, "Clients.Details", "تفاصيل عميل", "Clients", "Details", "العملاء", 33, false);
        await EnsurePermissionAsync(db, "Lawyers.View", "المحامون", "Lawyers", "Index", "المحامون", 40, true);
        await EnsurePermissionAsync(db, "Lawyers.Create", "إضافة محامي", "Lawyers", "Create", "المحامون", 41, false);
        await EnsurePermissionAsync(db, "Lawyers.Edit", "تعديل محامي", "Lawyers", "Edit", "المحامون", 42, false);
        await EnsurePermissionAsync(db, "Courts.View", "المحاكم", "Courts", "Index", "الإعدادات", 50, true);
        await EnsurePermissionAsync(db, "Courts.Create", "إضافة محكمة", "Courts", "Create", "الإعدادات", 51, false);
        await EnsurePermissionAsync(db, "Courts.Edit", "تعديل محكمة", "Courts", "Edit", "الإعدادات", 52, false);
        await EnsurePermissionAsync(db, "Lookups.View", "أنواع الـ Lookups", "Lookups", "Index", "الإعدادات", 60, true);
        await EnsurePermissionAsync(db, "Lookups.Items", "عناصر الـ Lookups", "Lookups", "Items", "الإعدادات", 61, false);
        await EnsurePermissionAsync(db, "Lookups.CreateType", "إضافة نوع Lookup", "Lookups", "CreateType", "الإعدادات", 62, false);
        await EnsurePermissionAsync(db, "Lookups.EditType", "تعديل نوع Lookup", "Lookups", "EditType", "الإعدادات", 63, false);
        await EnsurePermissionAsync(db, "Lookups.Create", "إضافة Lookup", "Lookups", "Create", "الإعدادات", 64, false);
        await EnsurePermissionAsync(db, "Lookups.Edit", "تعديل Lookup", "Lookups", "Edit", "الإعدادات", 65, false);
        await EnsurePermissionAsync(db, "Lookups.ToggleStatus", "تفعيل/تعطيل Lookup", "Lookups", "ToggleStatus", "الإعدادات", 66, false);
        await EnsurePermissionAsync(db, "Lookups.Delete", "حذف Lookup", "Lookups", "Delete", "الإعدادات", 67, false);
        await EnsurePermissionAsync(db, "Lookups.ToggleTypeStatus", "تفعيل/تعطيل نوع Lookup", "Lookups", "ToggleTypeStatus", "الإعدادات", 68, false);
        await EnsurePermissionAsync(db, "Lookups.DeleteType", "حذف نوع Lookup", "Lookups", "DeleteType", "الإعدادات", 69, false);
        await EnsurePermissionAsync(db, "Permissions.Index", "إدارة الصلاحيات", "Permissions", "Index", "الإعدادات", 80, true);
        await EnsurePermissionAsync(db, "Permissions.EditRole", "تعديل صلاحيات Role", "Permissions", "EditRole", "الإعدادات", 81, false);
        await EnsurePermissionAsync(db, "Users.Index", "إدارة المستخدمين", "Users", "Index", "الإعدادات", 82, true);
        await EnsurePermissionAsync(db, "Users.Create", "إضافة مستخدم", "Users", "Create", "الإعدادات", 83, false);
        await EnsurePermissionAsync(db, "Users.Edit", "تعديل مستخدم", "Users", "Edit", "الإعدادات", 84, false);
        await EnsurePermissionAsync(db, "Users.ToggleStatus", "تفعيل/تعطيل مستخدم", "Users", "ToggleStatus", "الإعدادات", 85, false);
        await EnsurePermissionAsync(db, "Payments.Create", "إضافة دفعة", "Payments", "Create", "القضايا", 90, false);
        await EnsurePermissionAsync(db, "Hearings.Create", "إضافة جلسة", "Hearings", "Create", "القضايا", 91, false);
        await EnsurePermissionAsync(db, "Hearings.Edit", "تعديل جلسة", "Hearings", "Edit", "القضايا", 92, false);
        await EnsurePermissionAsync(db, "Documents.Create", "إضافة مستند", "Documents", "Create", "القضايا", 93, false);
        await EnsurePermissionAsync(db, "Messages.Create", "إرسال رسالة", "Messages", "Create", "القضايا", 94, false);
        await EnsurePermissionAsync(db, "Notifications.View", "عرض الإشعارات", "Notifications", "Index", "الرئيسية", 95, false);
        await EnsurePermissionAsync(db, "Notifications.Open", "فتح الإشعار", "Notifications", "Open", "الرئيسية", 96, false);
        await EnsurePermissionAsync(db, "Reports.Overview", "التقارير العامة", "Reports", "Overview", "التقارير", 100, true);
        await EnsurePermissionAsync(db, "Reports.Lawyers", "تقارير المحامين", "Reports", "Lawyers", "التقارير", 101, true);
        await EnsurePermissionAsync(db, "Reports.Distribution", "توزيع القضايا", "Reports", "Distribution", "التقارير", 102, true);
        await EnsurePermissionAsync(db, "Reports.Export", "تصدير التقارير", "Reports", "Export", "التقارير", 103, false);

        await db.SaveChangesAsync();

        await GrantRolePermissionsAsync(db, "Admin", await db.SystemPermissions.Select(x => x.Id).ToListAsync());
        await GrantRolePermissionsAsync(db, "Manager", await db.SystemPermissions
            .Where(x => x.Code != "Permissions.Index" && x.Code != "Permissions.EditRole")
            .Select(x => x.Id)
            .ToListAsync());
        await GrantRolePermissionsAsync(db, "Secretary", await db.SystemPermissions
            .Where(x => x.Code == "Dashboard.View"
                || x.Code == "Dashboard.ViewAll"
                || x.Code == "Cases.View"
                || x.Code == "Cases.Create"
                || x.Code == "Cases.Edit"
                || x.Code == "Cases.Details"
                || x.Code == "Cases.ChangeStatus"
                || x.Code == "Clients.View"
                || x.Code == "Clients.Create"
                || x.Code == "Clients.Edit"
                || x.Code == "Clients.Details"
                || x.Code == "Payments.Create"
                || x.Code == "Hearings.Create"
                || x.Code == "Hearings.Edit"
                || x.Code == "Documents.Create"
                || x.Code == "Notifications.View"
                || x.Code == "Notifications.Open")
            .Select(x => x.Id)
            .ToListAsync());
        await GrantRolePermissionsAsync(db, "Lawyer", await db.SystemPermissions
            .Where(x => x.Code == "Dashboard.View"
                || x.Code == "Cases.View"
                || x.Code == "Cases.Details"
                || x.Code == "Documents.Create"
                || x.Code == "Hearings.Create"
                || x.Code == "Notifications.View"
                || x.Code == "Notifications.Open")
            .Select(x => x.Id)
            .ToListAsync());
        await GrantRolePermissionsAsync(db, "HR", await db.SystemPermissions
            .Where(x => x.Code == "Dashboard.View"
                || x.Code == "Notifications.View"
                || x.Code == "Notifications.Open")
            .Select(x => x.Id)
            .ToListAsync());

        if (!await db.MessageTemplates.AnyAsync())
        {
            db.MessageTemplates.AddRange(
                new MessageTemplate { Name = "تحديث حالة قضية", Channel = "WhatsApp", Body = "عميلنا العزيز {ClientName}، نحيطكم علمًا أن حالة القضية رقم {CaseNumber} أصبحت: {CaseStatus}." },
                new MessageTemplate { Name = "تذكير جلسة", Channel = "WhatsApp", Body = "تذكير: لديك جلسة للقضية رقم {CaseNumber} بتاريخ {HearingDate}." },
                new MessageTemplate { Name = "طلب مستندات", Channel = "SMS", Body = "برجاء إرسال المستندات المطلوبة للقضية رقم {CaseNumber}." }
            );
        }

        await db.SaveChangesAsync();
    }

    private static async Task EnsureLookupTypeAsync(AppDbContext db, string code, string nameAr, string? nameEn, int sortOrder)
    {
        var lookupType = await db.LookupTypes.FirstOrDefaultAsync(x => x.Code == code);
        if (lookupType == null)
        {
            db.LookupTypes.Add(new LookupType
            {
                Code = code,
                NameAr = nameAr,
                NameEn = nameEn,
                SortOrder = sortOrder,
                IsActive = true
            });
            return;
        }

        lookupType.NameAr = nameAr;
        lookupType.NameEn = nameEn;
        lookupType.SortOrder = sortOrder;
        lookupType.IsActive = true;
    }

    private static async Task EnsureLookupGroupAsync(AppDbContext db, string type, IEnumerable<(string Arabic, string English)> items)
    {
        var lookupType = db.LookupTypes.Local.FirstOrDefault(x => x.Code == type)
            ?? await db.LookupTypes.FirstOrDefaultAsync(x => x.Code == type);
        if (lookupType == null)
        {
            lookupType = new LookupType
            {
                Code = type,
                NameAr = type,
                NameEn = type,
                IsActive = true,
                SortOrder = 999
            };
            db.LookupTypes.Add(lookupType);
        }

        if (lookupType.Id == 0)
        {
            await db.SaveChangesAsync();
        }

        foreach (var (arabic, english) in items)
        {
            var exists = await db.Lookups.AnyAsync(x => x.LookupTypeId == lookupType.Id && x.NameAr == arabic);
            if (!exists)
            {
                db.Lookups.Add(new Lookup
                {
                    Type = type,
                    LookupTypeId = lookupType.Id,
                    LookupType = lookupType,
                    NameAr = arabic,
                    NameEn = english,
                    IsActive = true
                });
            }
        }
    }

    private static async Task EnsurePermissionAsync(AppDbContext db, string code, string nameAr, string controller, string action, string menuGroup, int sortOrder, bool isMenuItem)
    {
        var permission = await db.SystemPermissions.FirstOrDefaultAsync(x => x.Code == code);
        if (permission == null)
        {
            db.SystemPermissions.Add(new SystemPermission
            {
                Code = code,
                NameAr = nameAr,
                NameEn = code,
                Controller = controller,
                Action = action,
                MenuGroup = menuGroup,
                SortOrder = sortOrder,
                IsMenuItem = isMenuItem,
                IsActive = true
            });
            return;
        }

        permission.NameAr = nameAr;
        permission.NameEn = code;
        permission.Controller = controller;
        permission.Action = action;
        permission.MenuGroup = menuGroup;
        permission.SortOrder = sortOrder;
        permission.IsMenuItem = isMenuItem;
        permission.IsActive = true;
    }

    private static async Task GrantRolePermissionsAsync(AppDbContext db, string roleName, IReadOnlyCollection<int> permissionIds)
    {
        var existing = await db.RolePermissions.Where(x => x.RoleName == roleName).ToListAsync();
        db.RolePermissions.RemoveRange(existing);

        foreach (var permissionId in permissionIds)
        {
            db.RolePermissions.Add(new RolePermission
            {
                RoleName = roleName,
                SystemPermissionId = permissionId
            });
        }
    }
}
