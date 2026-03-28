using System.Security.Claims;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Roles.DTOs;

namespace FastFuel.Features.Roles.Services;

public interface IRoleService : ICrudService<RoleRequestDto, RoleResponseDto>
{
    Task<List<RoleResponseDto>> GetRolesForCurrentUserAsync(ClaimsPrincipal user,
        CancellationToken cancellationToken = default);
}