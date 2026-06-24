using LegalOffice.Infrastructure.Persistence;
using LegalOffice.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace LegalOffice.Web.Filters;

public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly AppDbContext _db;
    private readonly IPermissionService _permissions;

    public PermissionAuthorizationFilter(AppDbContext db, IPermissionService permissions)
    {
        _db = db;
        _permissions = permissions;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.Filters.OfType<IAllowAnonymousFilter>().Any())
        {
            return;
        }

        if (context.ActionDescriptor is not ControllerActionDescriptor descriptor)
        {
            return;
        }

        var controller = descriptor.ControllerName;
        var action = descriptor.ActionName;

        var permission = await _db.SystemPermissions.AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Controller == controller &&
                x.Action == action &&
                x.IsActive);

        if (permission == null)
        {
            return;
        }

        if (!await _permissions.HasPermissionAsync(context.HttpContext.User, permission.Code))
        {
            context.Result = new ForbidResult();
        }
    }
}
