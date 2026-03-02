using System.Reflection;
using System.Security.Claims;
using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Permissions.Services;

public class PermissionService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
    : IPermissionService
{
    private readonly IReadOnlyList<string> _permissions = GetAllPermissionNames();

    public Task<List<string>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_permissions.ToList());
    }

    public async Task<List<string>> GetPermissionsForCurrentUserAsync(ClaimsPrincipal user)
    {
        var appUser = await userManager.GetUserAsync(user);
        if (appUser == null) return [];

        var userClaims = await userManager.GetClaimsAsync(appUser);
        var userRoles = await userManager.GetRolesAsync(appUser);

        var roleClaims = new List<Claim>();
        foreach (var roleName in userRoles)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
                roleClaims.AddRange(await roleManager.GetClaimsAsync(role));
        }

        return userClaims.Concat(roleClaims)
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .Distinct()
            .OrderBy(p => p)
            .ToList();
    }

    private static List<string> GetAllPermissionNames()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(IsConcreteController)
            .SelectMany(GetPermissionsForController)
            .Distinct()
            .ToList();
    }

    private static IEnumerable<string> GetPermissionsForController(Type controllerType)
    {
        var controllerName = GetControllerName(controllerType);

        return controllerType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute<SkipPermissionCheckAttribute>() == null
                        && !m.GetCustomAttributes().OfType<IAllowAnonymous>().Any())
            .Select(m => m.GetCustomAttribute<PermissionCheckAttribute>())
            .Where(attr => attr != null)
            .Select(attr => PermissionParser.ParsePermissionName(controllerName, attr!.Operation));
    }

    private static bool IsConcreteController(Type type)
    {
        return type is { IsAbstract: false, IsInterface: false }
               && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetControllerName(Type type)
    {
        const string suffix = "Controller";
        return type.Name[..^suffix.Length];
    }
}