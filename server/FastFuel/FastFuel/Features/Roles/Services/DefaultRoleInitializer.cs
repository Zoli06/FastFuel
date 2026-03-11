using System.Security.Claims;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Roles.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Roles.Services;

public class DefaultRoleInitializer(RoleManager<Role> roleManager) : IDefaultRoleInitializer
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
                "Permission:Restaurant:Read"
            ],
            [DefaultRole.Customer] =
            [
                "Permission:Order:Create"
            ],
            [DefaultRole.Employee] =
            [
                "Permission:Shift:Read",
                "Permission:StationCategory:Read",
                "Permission:Employee:Read",
                "Permission:Order:Create",
                "Permission:Order:Read",
                "Permission:Order:Update",
                "Permission:Order:Delete",
                "Permission:Station:Read",
                "Permission:Customer:Read",
                "Permission:Station:ViewTasks"
            ]
        };

    public async Task InitializeAsync()
    {
        foreach (var (defaultRole, permissions) in DefaultRoles)
        {
            var roleName = defaultRole.ToRoleName();
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var role = new Role { Name = roleName, IsDefault = true };
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
}