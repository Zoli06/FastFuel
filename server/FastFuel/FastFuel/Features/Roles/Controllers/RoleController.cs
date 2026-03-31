using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Roles.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Roles.Controllers;

public class RoleController(IRoleService service)
    : CrudController<Role, RoleRequestDto, RoleResponseDto>(service)
{
    /// <summary>
    /// Gets the roles assigned to the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current user's roles.</returns>
    [HttpGet("my")]
    public async Task<Results<Ok<List<RoleResponseDto>>, UnauthorizedHttpResult>> GetMyRoles(
        CancellationToken cancellationToken = default)
    {
        return TypedResults.Ok(await service.GetRolesForCurrentUserAsync(User, cancellationToken));
    }
}