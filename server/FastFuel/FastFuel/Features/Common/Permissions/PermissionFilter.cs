using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.Permissions;

public class PermissionFilter(
    string operation,
    UserManager<User> userManager,
    IUserClaimsPrincipalFactory<User> claimsPrincipalFactory)
    : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
            return;

        if (context.ActionDescriptor.EndpointMetadata.OfType<SkipPermissionCheckAttribute>().Any())
            return;

        var user = context.HttpContext.User;
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new ChallengeResult();
            return;
        }

        var group = context.RouteData.Values["controller"]?.ToString();
        if (group == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var dbUser = await userManager.GetUserAsync(user);
        if (dbUser == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var principal = await claimsPrincipalFactory.CreateAsync(dbUser);

        var permissionName = PermissionParser.ParsePermissionName(group, operation);

        if (!principal.HasClaim("Permission", permissionName))
            context.Result = new ForbidResult();
    }
}