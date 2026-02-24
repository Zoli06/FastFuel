using System.Reflection;
using System.Security.Claims;
using FastFuel.Features.Common.Authorization;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Permissions.Services;

public class PermissionService : IPermissionService
{
    private readonly IReadOnlyList<string> _permissions;
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;

    public PermissionService(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        var operations = Enum.GetNames<PermissionType>();

        _permissions = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.IsAssignableTo(typeof(ControllerBase))
                        && t.GetMethods().Any(m => m.IsDefined(typeof(PermissionAuthorizeAttribute), true)))
            .Select(t => t.Name.Replace("Controller", ""))
            .SelectMany(resource => operations.Select(op => $"Permission:{resource}:{op}"))
            .OrderBy(p => p)
            .ToList()
            .AsReadOnly();

        _userManager = userManager;
        _roleManager = roleManager;
    }

    public List<string> GetAllPermissions()
    {
        return _permissions.ToList();
    }

    public async Task<List<string>> GetPermissionsForCurrentUserAsync(ClaimsPrincipal user)
    {
        var appUser = await _userManager.GetUserAsync(user);
        if (appUser == null) return [];

        var userClaims = await _userManager.GetClaimsAsync(appUser);
        var userRoles = await _userManager.GetRolesAsync(appUser);

        var roleClaims = new List<Claim>();
        foreach (var roleName in userRoles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
                roleClaims.AddRange(await _roleManager.GetClaimsAsync(role));
        }

        return userClaims.Concat(roleClaims)
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .Distinct()
            .OrderBy(p => p)
            .ToList();
    }
}