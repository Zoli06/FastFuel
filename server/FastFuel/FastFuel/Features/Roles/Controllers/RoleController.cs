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
    [HttpGet("my")]
    public async Task<Results<Ok<List<RoleResponseDto>>, UnauthorizedHttpResult>> GetMyRoles(
        CancellationToken cancellationToken = default)
    {
        return TypedResults.Ok(await service.GetRolesForCurrentUserAsync(User, cancellationToken));
    }
}