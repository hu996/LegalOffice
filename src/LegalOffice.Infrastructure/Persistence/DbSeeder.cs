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
        await EnsureLookupTypeAsync(db, "TaskStatus", "حالات المهام", "Task Statuses", 130);
        await EnsureLookupTypeAsync(db, "TaskPriority", "أولويات المهام", "Task Priorities", 140);
        await EnsureLookupTypeAsync(db, "TaskType", "أنواع المهام", "Task Types", 150);
        await EnsureLookupTypeAsync(db, "CaseWorkflowStage", "مراحل القضية", "Case Workflow Stages", 160);
        await EnsureLookupTypeAsync(db, "ConflictCheckStatus", "حالات تعارض المصالح", "Conflict Check Statuses", 170);
        await EnsureLookupTypeAsync(db, "ExpenseStatus", "حالات المصروفات", "Expense Statuses", 180);
        await EnsureLookupTypeAsync(db, "ConsultationType", "أنواع الاستشارات", "Consultation Types", 190);
        await EnsureLookupTypeAsync(db, "ConsultationStatus", "حالات الاستشارات", "Consultation Statuses", 200);
        await EnsureLookupTypeAsync(db, "ContractType", "أنواع العقود", "Contract Types", 210);
        await EnsureLookupTypeAsync(db, "ContractStatus", "حالات العقود", "Contract Statuses", 220);
        await EnsureLookupTypeAsync(db, "PowerType", "أنواع التوكيلات", "Power Types", 230);
        await EnsureLookupTypeAsync(db, "PowerStatus", "حالات التوكيلات", "Power Statuses", 240);
        await EnsureLookupTypeAsync(db, "CourtLevel", "درجات المحاكم", "Court Levels", 250);
        await EnsureLookupTypeAsync(db, "ExecutionStatus", "حالات التنفيذ", "Execution Statuses", 260);
        await EnsureLookupTypeAsync(db, "MeetingStatus", "حالات الاجتماعات", "Meeting Statuses", 270);
        await EnsureLookupTypeAsync(db, "FeeType", "أنواع الأتعاب", "Fee Types", 280);
        await EnsureLookupTypeAsync(db, "InstallmentStatus", "حالات الأقساط", "Installment Statuses", 290);
        await EnsureLookupTypeAsync(db, "TransactionType", "أنواع الحركات", "Transaction Types", 300);

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

        await EnsureLookupGroupAsync(db, "TaskStatus", new[]
        {
            ("جديدة", "New"),
            ("قيد التنفيذ", "InProgress"),
            ("مكتملة", "Completed"),
            ("متأخرة", "Overdue"),
            ("ملغاة", "Cancelled")
        });

        await EnsureLookupGroupAsync(db, "TaskPriority", new[]
        {
            ("منخفضة", "Low"),
            ("متوسطة", "Medium"),
            ("عالية", "High"),
            ("عاجلة", "Urgent")
        });

        await EnsureLookupGroupAsync(db, "TaskType", new[]
        {
            ("تحضير مذكرة", "Memo"),
            ("مراجعة عقد", "ContractReview"),
            ("رفع مستند", "UploadDocument"),
            ("متابعة جلسة", "HearingFollowUp"),
            ("اتصال بعميل", "ClientCall"),
            ("إجراء إداري", "AdminAction"),
            ("أخرى", "Other")
        });

        await EnsureLookupGroupAsync(db, "CaseWorkflowStage", new[]
        {
            ("قيد الدراسة", "Study"),
            ("تم فتح ملف", "Opened"),
            ("تم رفع الدعوى", "Filed"),
            ("منظورة أمام المحكمة", "InCourt"),
            ("محجوزة للحكم", "Reserved"),
            ("صدر حكم", "Judged"),
            ("في الاستئناف", "Appeal"),
            ("في التنفيذ", "Execution"),
            ("منتهية", "Closed"),
            ("مؤرشفة", "Archived")
        });

        await EnsureLookupGroupAsync(db, "ConflictCheckStatus", new[]
        {
            ("لا يوجد تعارض", "None"),
            ("يوجد تعارض محتمل", "Possible"),
            ("يوجد تعارض مؤكد", "Confirmed"),
            ("يحتاج مراجعة", "Review")
        });

        await EnsureLookupGroupAsync(db, "ExpenseStatus", new[]
        {
            ("مسودة", "Draft"),
            ("قيد المراجعة", "Review"),
            ("معتمد", "Approved"),
            ("مرفوض", "Rejected")
        });

        await EnsureLookupGroupAsync(db, "ConsultationType", new[]
        {
            ("استشارة فردية", "Individual"),
            ("استشارة شركات", "Corporate"),
            ("استشارة ضريبية", "Tax"),
            ("استشارة عقارية", "RealEstate"),
            ("استشارة عمالية", "Labor"),
            ("استشارة تجارية", "Commercial"),
            ("أخرى", "Other")
        });

        await EnsureLookupGroupAsync(db, "ConsultationStatus", new[]
        {
            ("جديدة", "New"),
            ("قيد الدراسة", "UnderReview"),
            ("بانتظار العميل", "WaitingClient"),
            ("تم الرد", "Responded"),
            ("مغلقة", "Closed")
        });

        await EnsureLookupGroupAsync(db, "ContractType", new[]
        {
            ("عقد بيع", "Sale"),
            ("عقد إيجار", "Lease"),
            ("عقد عمل", "Employment"),
            ("عقد شراكة", "Partnership"),
            ("عقد تأسيس شركة", "CompanyFormation"),
            ("عقد خدمات", "Services"),
            ("عقد توريد", "Supply"),
            ("عقد مقاولات", "Contracting"),
            ("أخرى", "Other")
        });

        await EnsureLookupGroupAsync(db, "ContractStatus", new[]
        {
            ("مسودة", "Draft"),
            ("تحت المراجعة", "Review"),
            ("معتمد", "Approved"),
            ("ساري", "Active"),
            ("منتهي", "Expired"),
            ("ملغي", "Cancelled")
        });

        await EnsureLookupGroupAsync(db, "PowerType", new[]
        {
            ("عام", "General"),
            ("خاص", "Special"),
            ("قضائي", "Judicial"),
            ("بنكي", "Bank"),
            ("عقاري", "RealEstate"),
            ("شركات", "Corporate")
        });

        await EnsureLookupGroupAsync(db, "PowerStatus", new[]
        {
            ("ساري", "Valid"),
            ("منتهي", "Expired"),
            ("ملغي", "Cancelled")
        });

        await EnsureLookupGroupAsync(db, "CourtLevel", new[]
        {
            ("أول درجة", "First"),
            ("استئناف", "Appeal"),
            ("نقض", "Cassation")
        });

        await EnsureLookupGroupAsync(db, "ExecutionStatus", new[]
        {
            ("لم يبدأ", "NotStarted"),
            ("جارى التنفيذ", "InProgress"),
            ("تم الإعلان", "Notified"),
            ("تم الحجز", "Seized"),
            ("تم التحصيل", "Collected"),
            ("متعثر", "Failed"),
            ("منتهي", "Closed")
        });

        await EnsureLookupGroupAsync(db, "MeetingStatus", new[]
        {
            ("مجدول", "Scheduled"),
            ("تم", "Done"),
            ("ملغي", "Cancelled"),
            ("مؤجل", "Deferred")
        });

        await EnsureLookupGroupAsync(db, "FeeType", new[]
        {
            ("مبلغ ثابت", "Fixed"),
            ("بالجلسة", "PerHearing"),
            ("نسبة من التحصيل", "Percentage"),
            ("عقد شهري", "Monthly"),
            ("مراحل", "Milestone")
        });

        await EnsureLookupGroupAsync(db, "InstallmentStatus", new[]
        {
            ("مستحق", "Due"),
            ("مدفوع", "Paid"),
            ("متأخر", "Late"),
            ("ملغي", "Cancelled")
        });

        await EnsureLookupGroupAsync(db, "TransactionType", new[]
        {
            ("إيراد", "Income"),
            ("مصروف", "Expense"),
            ("عهدة", "Custody"),
            ("تحويل", "Transfer")
        });

        await EnsureBranchAsync(db, "الفرع الرئيسي", "القاهرة", true);

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
        await EnsurePermissionAsync(db, "Tasks.View", "عرض المهام", "Tasks", "Index", "الإدارة", 110, true);
        await EnsurePermissionAsync(db, "Tasks.Create", "إضافة مهمة", "Tasks", "Create", "الإدارة", 111, false);
        await EnsurePermissionAsync(db, "Tasks.Edit", "تعديل مهمة", "Tasks", "Edit", "الإدارة", 112, false);
        await EnsurePermissionAsync(db, "Tasks.Delete", "حذف مهمة", "Tasks", "Delete", "الإدارة", 113, false);
        await EnsurePermissionAsync(db, "Tasks.ChangeStatus", "تغيير حالة المهمة", "Tasks", "ChangeStatus", "الإدارة", 114, false);
        await EnsurePermissionAsync(db, "Tasks.Close", "إغلاق المهمة", "Tasks", "Close", "الإدارة", 115, false);
        await EnsurePermissionAsync(db, "Tasks.ViewMine", "عرض مهامي", "Tasks", "Mine", "الإدارة", 116, true);
        await EnsurePermissionAsync(db, "Tasks.ViewToday", "عرض مهام اليوم", "Tasks", "Today", "الإدارة", 117, true);
        await EnsurePermissionAsync(db, "Tasks.ViewLate", "عرض المهام المتأخرة", "Tasks", "Late", "الإدارة", 118, true);
        await EnsurePermissionAsync(db, "AuditLogs.View", "عرض سجل التدقيق", "AuditLogs", "Index", "الإدارة", 119, true);
        await EnsurePermissionAsync(db, "Branches.View", "عرض الفروع", "Branches", "Index", "الإدارة", 120, true);
        await EnsurePermissionAsync(db, "Branches.Create", "إضافة فرع", "Branches", "Create", "الإدارة", 121, false);
        await EnsurePermissionAsync(db, "Branches.Edit", "تعديل فرع", "Branches", "Edit", "الإدارة", 122, false);
        await EnsurePermissionAsync(db, "Branches.Delete", "حذف فرع", "Branches", "Delete", "الإدارة", 123, false);
        await EnsurePermissionAsync(db, "Departments.View", "عرض الأقسام", "Departments", "Index", "الإدارة", 124, true);
        await EnsurePermissionAsync(db, "Departments.Create", "إضافة قسم", "Departments", "Create", "الإدارة", 125, false);
        await EnsurePermissionAsync(db, "Departments.Edit", "تعديل قسم", "Departments", "Edit", "الإدارة", 126, false);
        await EnsurePermissionAsync(db, "Departments.Delete", "حذف قسم", "Departments", "Delete", "الإدارة", 127, false);
        await EnsurePermissionAsync(db, "ConflictChecks.View", "فحص تعارض المصالح", "ConflictChecks", "Index", "الإدارة", 128, true);
        await EnsurePermissionAsync(db, "ConflictChecks.Create", "إضافة فحص تعارض", "ConflictChecks", "Create", "الإدارة", 129, false);
        await EnsurePermissionAsync(db, "InternalNotes.View", "عرض الملاحظات الداخلية", "CaseNotes", "Index", "القضايا", 130, false);
        await EnsurePermissionAsync(db, "InternalNotes.Create", "إضافة ملاحظة داخلية", "CaseNotes", "Create", "القضايا", 131, false);
        await EnsurePermissionAsync(db, "InternalNotes.Edit", "تعديل ملاحظة داخلية", "CaseNotes", "Edit", "القضايا", 132, false);
        await EnsurePermissionAsync(db, "InternalNotes.Delete", "حذف ملاحظة داخلية", "CaseNotes", "Delete", "القضايا", 133, false);
        await EnsurePermissionAsync(db, "CaseWorkflow.ChangeStage", "تغيير مرحلة القضية", "Cases", "ChangeStage", "القضايا", 134, false);
        await EnsurePermissionAsync(db, "CaseAssignmentHistory.View", "عرض سجل إسناد المحامين", "Cases", "AssignmentHistory", "القضايا", 135, false);
        await EnsurePermissionAsync(db, "Expenses.Submit", "إرسال المصروفات للمراجعة", "Expenses", "Submit", "المالية", 136, false);
        await EnsurePermissionAsync(db, "Expenses.Approve", "اعتماد المصروفات", "Expenses", "Approve", "المالية", 137, false);
        await EnsurePermissionAsync(db, "Expenses.Reject", "رفض المصروفات", "Expenses", "Reject", "المالية", 138, false);
        await EnsurePermissionAsync(db, "Consultations.View", "عرض الاستشارات", "Consultations", "Index", "الاستشارات", 139, true);
        await EnsurePermissionAsync(db, "Consultations.Create", "إضافة استشارة", "Consultations", "Create", "الاستشارات", 140, false);
        await EnsurePermissionAsync(db, "Consultations.Edit", "تعديل استشارة", "Consultations", "Edit", "الاستشارات", 141, false);
        await EnsurePermissionAsync(db, "Consultations.Assign", "تعيين مستشار", "Consultations", "Assign", "الاستشارات", 142, false);
        await EnsurePermissionAsync(db, "Consultations.Opinions", "تسجيل الرأي القانوني", "Consultations", "Opinion", "الاستشارات", 143, false);
        await EnsurePermissionAsync(db, "Consultations.Attachments", "رفع مستندات الاستشارة", "Consultations", "Attachments", "الاستشارات", 144, false);
        await EnsurePermissionAsync(db, "Contracts.View", "عرض العقود", "Contracts", "Index", "العقود", 145, true);
        await EnsurePermissionAsync(db, "Contracts.Create", "إضافة عقد", "Contracts", "Create", "العقود", 146, false);
        await EnsurePermissionAsync(db, "Contracts.Edit", "تعديل عقد", "Contracts", "Edit", "العقود", 147, false);
        await EnsurePermissionAsync(db, "Contracts.Version", "إدارة نسخ العقد", "Contracts", "Versions", "العقود", 148, false);
        await EnsurePermissionAsync(db, "PowerOfAttorney.View", "عرض التوكيلات", "PowerOfAttorney", "Index", "التوكيلات", 149, true);
        await EnsurePermissionAsync(db, "PowerOfAttorney.Create", "إضافة توكيل", "PowerOfAttorney", "Create", "التوكيلات", 150, false);
        await EnsurePermissionAsync(db, "PowerOfAttorney.Edit", "تعديل توكيل", "PowerOfAttorney", "Edit", "التوكيلات", 151, false);
        await EnsurePermissionAsync(db, "PowerOfAttorney.Upload", "رفع ملف التوكيل", "PowerOfAttorney", "Upload", "التوكيلات", 152, false);
        await EnsurePermissionAsync(db, "Judgments.View", "عرض الأحكام", "Judgments", "Index", "الأحكام والتنفيذ", 153, true);
        await EnsurePermissionAsync(db, "Judgments.Create", "تسجيل حكم", "Judgments", "Create", "الأحكام والتنفيذ", 154, false);
        await EnsurePermissionAsync(db, "Execution.View", "عرض التنفيذ", "Execution", "Index", "الأحكام والتنفيذ", 155, true);
        await EnsurePermissionAsync(db, "Execution.Create", "إضافة ملف تنفيذ", "Execution", "Create", "الأحكام والتنفيذ", 156, false);
        await EnsurePermissionAsync(db, "Meetings.View", "عرض الاجتماعات", "Meetings", "Index", "الاجتماعات", 157, true);
        await EnsurePermissionAsync(db, "Meetings.Create", "إضافة اجتماع", "Meetings", "Create", "الاجتماعات", 158, false);
        await EnsurePermissionAsync(db, "Meetings.Edit", "تعديل اجتماع", "Meetings", "Edit", "الاجتماعات", 159, false);
        await EnsurePermissionAsync(db, "Treasury.View", "عرض خزنة المكتب", "Treasury", "Index", "الخزنة", 160, true);
        await EnsurePermissionAsync(db, "Treasury.Create", "إضافة حركة خزنة", "Treasury", "Create", "الخزنة", 161, false);
        await EnsurePermissionAsync(db, "Templates.View", "عرض القوالب", "Templates", "Index", "الإعدادات", 162, true);
        await EnsurePermissionAsync(db, "Templates.Create", "إضافة قالب", "Templates", "Create", "الإعدادات", 163, false);
        await EnsurePermissionAsync(db, "Templates.Edit", "تعديل قالب", "Templates", "Edit", "الإعدادات", 164, false);
        await EnsurePermissionAsync(db, "ImportExport.View", "الاستيراد والتصدير", "DataExchange", "Index", "الإعدادات", 165, true);
        await EnsurePermissionAsync(db, "Backup.View", "النسخ الاحتياطي", "Backup", "Index", "الإعدادات", 166, true);

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
                || x.Code == "Notifications.Open"
                || x.Code == "Tasks.View"
                || x.Code == "Tasks.ViewMine"
                || x.Code == "Tasks.ViewToday"
                || x.Code == "Tasks.Create"
                || x.Code == "Tasks.ChangeStatus"
                || x.Code == "Tasks.Close")
            .Select(x => x.Id)
            .ToListAsync());
        await GrantRolePermissionsAsync(db, "Lawyer", await db.SystemPermissions
            .Where(x => x.Code == "Dashboard.View"
                || x.Code == "Cases.View"
                || x.Code == "Cases.Details"
                || x.Code == "Documents.Create"
                || x.Code == "Hearings.Create"
                || x.Code == "Notifications.View"
                || x.Code == "Notifications.Open"
                || x.Code == "Tasks.ViewMine"
                || x.Code == "Tasks.ViewToday")
            .Select(x => x.Id)
            .ToListAsync());
        await GrantRolePermissionsAsync(db, "HR", await db.SystemPermissions
            .Where(x => x.Code == "Dashboard.View"
                || x.Code == "Notifications.View"
                || x.Code == "Notifications.Open"
                || x.Code == "Departments.View"
                || x.Code == "Tasks.ViewMine"
                || x.Code == "Tasks.ViewToday")
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

    private static async Task EnsureBranchAsync(AppDbContext db, string nameAr, string? address, bool isActive)
    {
        var exists = await db.Branches.AnyAsync(x => x.NameAr == nameAr);
        if (exists)
        {
            return;
        }

        db.Branches.Add(new Branch
        {
            NameAr = nameAr,
            Address = address,
            IsActive = isActive
        });
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
