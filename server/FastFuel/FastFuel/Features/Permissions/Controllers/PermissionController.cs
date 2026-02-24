using FastFuel.Features.Common.Authorization;
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
    [HttpGet]
    [CrudAuthorize(PermissionType.Read)]
    public Results<
            Ok<List<string>>,
            UnauthorizedHttpResult,
            ForbidHttpResult>
        GetAll()
    {
        return TypedResults.Ok(permissionService.GetAllPermissions());
    }

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