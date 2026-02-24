using System.Security.Claims;

namespace FastFuel.Features.Permissions.Services;

public interface IPermissionService
{
    List<string> GetAllPermissions();
    Task<List<string>> GetPermissionsForCurrentUserAsync(ClaimsPrincipal user);
}