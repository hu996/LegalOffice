using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LegalOffice.Web.Services;

public interface IWorkflowStatusService
{
    Task<int?> GetInitialStatusIdAsync(string lookupType);

    Task<List<SelectListItem>> GetSequentialOptionsAsync(string lookupType, int? currentStatusId);

    Task<bool> ValidateSequentialTransitionAsync(
        string lookupType,
        int? currentStatusId,
        int selectedStatusId,
        ModelStateDictionary modelState,
        string fieldName,
        string entityLabel);
}
