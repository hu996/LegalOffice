using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class TreasuryDashboardVM
{
    public int TreasuryId { get; set; }
    public string TreasuryName { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public IEnumerable<TreasuryTransactionListItemVM> Transactions { get; set; } = new List<TreasuryTransactionListItemVM>();
}

public class TreasuryTransactionListItemVM
{
    public int Id { get; set; }
    public string TransactionTypeName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? CaseNumber { get; set; }
}

public class TreasuryTransactionVM
{
    [Display(Name = "نوع القضية")]
    public int? CaseTypeId { get; set; }

    [Required(ErrorMessage = "الخزنة مطلوبة.")]
    [Display(Name = "الخزنة")]
    public int TreasuryId { get; set; }

    [Required(ErrorMessage = "نوع الحركة مطلوب.")]
    [Display(Name = "نوع الحركة")]
    public int TransactionTypeLookupId { get; set; }

    [Required(ErrorMessage = "المبلغ مطلوب.")]
    [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر.")]
    [Display(Name = "المبلغ")]
    public decimal Amount { get; set; }

    [Display(Name = "ملاحظات")]
    [StringLength(2000, ErrorMessage = "الملاحظات لا يجوز أن تتجاوز 2000 حرف.")]
    public string? Notes { get; set; }

    [Display(Name = "تاريخ الحركة")]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    [Display(Name = "القضية المرتبطة")]
    public int? CaseId { get; set; }

    public IEnumerable<SelectListItem> Treasuries { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> TransactionTypes { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Cases { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> CaseTypes { get; set; } = new List<SelectListItem>();
}
