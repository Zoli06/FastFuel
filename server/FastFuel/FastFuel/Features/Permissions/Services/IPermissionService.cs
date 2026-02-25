using System.Security.Claims;

namespace FastFuel.Features.Permissions.Services;

public interface IPermissionService
{
    Task<List<string>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetPermissionsForCurrentUserAsync(ClaimsPrincipal user);
}