using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace LegalOffice.Web.Services;

public interface ICaseTypeOptionsService
{
    Task<List<SelectListItem>> GetVisibleCaseTypesAsync(ClaimsPrincipal user);
}
