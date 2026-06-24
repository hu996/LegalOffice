using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LegalOffice.Application.ViewModels;

public class PaymentCreateEditVM
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "القضية مطلوبة.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار القضية.")]
    [Display(Name = "القضية")]
    public int CaseId { get; set; }

    [Required(ErrorMessage = "المبلغ مطلوب.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر.")]
    [Display(Name = "المبلغ")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "تاريخ السداد مطلوب.")]
    [Display(Name = "تاريخ السداد")]
    public DateTime PaymentDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "حالة السداد مطلوبة.")]
    [Range(1, int.MaxValue, ErrorMessage = "يجب اختيار حالة السداد.")]
    [Display(Name = "حالة السداد")]
    public int PaymentStatusId { get; set; }

    [Display(Name = "طريقة السداد")]
    public int? PaymentMethodId { get; set; }

    [StringLength(100, ErrorMessage = "رقم المرجع لا يجب أن يزيد عن 100 حرف.")]
    [Display(Name = "رقم المرجع")]
    public string? ReferenceNumber { get; set; }

    [StringLength(1000, ErrorMessage = "الملاحظات لا يجب أن تزيد عن 1000 حرف.")]
    [Display(Name = "الملاحظات")]
    public string? Notes { get; set; }

    public IEnumerable<SelectListItem> PaymentStatuses { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> PaymentMethods { get; set; } = new List<SelectListItem>();
}
