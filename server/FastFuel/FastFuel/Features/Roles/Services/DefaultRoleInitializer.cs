using System.Security.Claims;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Permissions.Services;
using FastFuel.Features.Roles.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Roles.Services;

public class DefaultRoleInitializer(RoleManager<Role> roleManager, IPermissionService permissionService)
    : IDefaultRoleInitializer
{
    private static readonly IReadOnlyDictionary<DefaultRole, string[]> DefaultRoles =
        new Dictionary<DefaultRole, string[]>
        {
            [DefaultRole.User] =
            [
                "Permission:Menu:Read",
                "Permission:Food:Read",
                "Permission:Ingredient:Read",
                "Permission:Allergy:Read",
                "Permission:Restaurant:Read",
                "Permission:Order:ReadOwn",
                "Permission:Restaurant:Read"
            ],
            [DefaultRole.Customer] =
            [
                "Permission:Order:Create",
                "Permission:Customer:UpdateSelf"
            ],
            [DefaultRole.Employee] =
            [
                "Permission:Shift:ReadOwn",
                "Permission:StationCategory:Read",
                "Permission:Employee:ReadOwn",
                "Permission:Order:CreateAtWorkplace",
                "Permission:Order:Read",
                "Permission:Order:UpdateStatus",
                "Permission:Station:Read",
                "Permission:Station:ViewTasks"
            ],
            [DefaultRole.Machine] =
            [
            ]
        };

    public async Task InitializeAsync()
    {
        await InitializeAdminRoleAsync();

        foreach (var (defaultRole, permissions) in DefaultRoles)
        {
            var roleName = defaultRole.ToRoleName();
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var role = new Role { Name = roleName, IsDefault = true, IsImmutable = false };
            var roleResult = await roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
                throw new ValidationAppException(
                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));

            foreach (var permission in permissions)
            {
                var claimResult = await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                if (!claimResult.Succeeded)
                    throw new ValidationAppException(
                        string.Join("; ", claimResult.Errors.Select(e => e.Description)));
            }
        }
    }

    private async Task InitializeAdminRoleAsync()
    {
        var adminRoleName = DefaultRole.Admin.ToRoleName();
        var adminRole = await roleManager.FindByNameAsync(adminRoleName);
        if (adminRole == null)
        {
            adminRole = new Role { Name = adminRoleName, IsDefault = true, IsImmutable = true };
            var createResult = await roleManager.CreateAsync(adminRole);
            if (!createResult.Succeeded)
                throw new ValidationAppException(
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
        }

        var shouldUpdate = false;
        if (!adminRole.IsDefault)
        {
            adminRole.IsDefault = true;
            shouldUpdate = true;
        }

        if (!adminRole.IsImmutable)
        {
            adminRole.IsImmutable = true;
            shouldUpdate = true;
        }

        if (shouldUpdate)
        {
            var updateResult = await roleManager.UpdateAsync(adminRole);
            if (!updateResult.Succeeded)
                throw new ValidationAppException(
                    string.Join("; ", updateResult.Errors.Select(e => e.Description)));
        }

        var allPermissions = await permissionService.GetAllPermissionsAsync();
        var currentPermissionClaims = (await roleManager.GetClaimsAsync(adminRole))
            .Where(claim => claim.Type == "Permission")
            .Select(claim => claim.Value)
            .ToHashSet();

        var requiredPermissions = allPermissions.ToHashSet();

        foreach (var permission in requiredPermissions.Except(currentPermissionClaims))
        {
            var claimResult = await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
            if (!claimResult.Succeeded)
                throw new ValidationAppException(
                    string.Join("; ", claimResult.Errors.Select(e => e.Description)));
        }

        foreach (var permission in currentPermissionClaims.Except(requiredPermissions))
        {
            var claimResult = await roleManager.RemoveClaimAsync(adminRole, new Claim("Permission", permission));
            if (!claimResult.Succeeded)
                throw new ValidationAppException(
                    string.Join("; ", claimResult.Errors.Select(e => e.Description)));
        }
    }
}