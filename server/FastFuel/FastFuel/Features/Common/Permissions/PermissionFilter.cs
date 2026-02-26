using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.Permissions;

public class PermissionFilter(string operation) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
            return;

        if (context.ActionDescriptor.EndpointMetadata.OfType<SkipPermissionCheckAttribute>().Any())
            return;

        var group = context.RouteData.Values["controller"]?.ToString();
        if (group == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var permissionName = PermissionParser.ParsePermissionName(group, operation);

        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        if (!user.HasClaim("Permission", permissionName)) context.Result = new ForbidResult();
    }
}