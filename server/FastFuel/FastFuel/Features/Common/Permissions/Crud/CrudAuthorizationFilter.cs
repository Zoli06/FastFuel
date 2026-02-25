using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.Permissions.Crud;

public class CrudAuthorizationFilter(CrudPermissionType crudPermission) : IAuthorizationFilter
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

        var requiredClaim = crudPermission switch
        {
            CrudPermissionType.Create => CrudPermissions.Create(controllerName),
            CrudPermissionType.Read => CrudPermissions.Read(controllerName),
            CrudPermissionType.Update => CrudPermissions.Update(controllerName),
            CrudPermissionType.Delete => CrudPermissions.Delete(controllerName),
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