using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.Authorization;

public class CrudAuthorizationFilter(PermissionType permission) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
            return;

        if (context.ActionDescriptor.EndpointMetadata.OfType<SkipPermissionCheckAttribute>().Any())
            return;

        var controllerName = context.RouteData.Values["controller"]?.ToString();
        if (controllerName == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var requiredClaim = permission switch
        {
            PermissionType.Create => Permissions.Create(controllerName),
            PermissionType.Read => Permissions.Read(controllerName),
            PermissionType.Update => Permissions.Update(controllerName),
            PermissionType.Delete => Permissions.Delete(controllerName),
            _ => throw new ArgumentOutOfRangeException()
        };

        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        if (!user.HasClaim("Permission", requiredClaim)) context.Result = new ForbidResult();
    }
}