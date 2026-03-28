using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Permissions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Permissions.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PermissionController(IPermissionService permissionService) : ControllerBase
{
    /// <summary>
    /// Gets all permissions available in the system.
    /// </summary>
    /// <returns>The full list of permission names.</returns>
    [HttpGet]
    [PermissionCheck(CrudOperation.Read)]
    public async Task<Results<
            Ok<List<string>>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        GetAll()
    {
        return TypedResults.Ok(await permissionService.GetAllPermissionsAsync());
    }

    /// <summary>
    /// Gets the permissions of the currently authenticated user.
    /// </summary>
    /// <returns>The current user's permission names.</returns>
    [HttpGet("my")]
    public async Task<Results<
            Ok<List<string>>,
            UnauthorizedHttpResult>>
        GetMyPermissions()
    {
        var permissions = await permissionService.GetPermissionsForCurrentUserAsync(User);
        return TypedResults.Ok(permissions);
    }
}