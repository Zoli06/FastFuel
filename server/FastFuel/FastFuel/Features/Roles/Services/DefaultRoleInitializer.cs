using System.Security.Claims;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Permissions.Services;
using FastFuel.Features.Roles.Common;
using FastFuel.Features.Roles.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Roles.Services;

public class DefaultRoleInitializer(RoleManager<Role> roleManager, IPermissionService permissionService)
    : IDefaultRoleInitializer
{
    private static readonly IReadOnlyDictionary<DefaultRole, string[]> DefaultRoles =
        new Dictionary<DefaultRole, string[]>
        {
            [DefaultRole.Customer] =
            [
                "Permission:Menu:Read",
                "Permission:Food:Read",
                "Permission:Ingredient:Read",
                "Permission:Allergy:Read",
                "Permission:Restaurant:Read",
                "Permission:Restaurant:Read",
                "Permission:Order:Create",
                "Permission:Customer:UpdateSelf"
            ],
            [DefaultRole.Employee] =
            [
                "Permission:Menu:Read",
                "Permission:Food:Read",
                "Permission:Ingredient:Read",
                "Permission:Allergy:Read",
                "Permission:Restaurant:Read",
                "Permission:Restaurant:Read",
                "Permission:StationCategory:Read",
                "Permission:Order:Create",
                "Permission:Order:UpdateStatus",
                "Permission:Station:Read",
                "Permission:Station:ViewTasks"
            ],
            [DefaultRole.Machine] =
            [
                "Permission:Order:Read",
                "Permission:Order:Create",
                "Permission:Restaurant:Read",
                "Permission:Food:Read",
                "Permission:Menu:Read",
                "Permission:Station:ViewTasks",
                "Permission:Station:Read",
                "Permission:Order:UpdateStatus"
            ]
        };

    private static readonly IReadOnlyDictionary<DefaultRole, Page[]> DefaultRolePages =
        new Dictionary<DefaultRole, Page[]>
        {
            [DefaultRole.Admin] =
            [
                Page.AdminManager, Page.AllergyManager, Page.CustomerManager, Page.IngredientManager,
                Page.EmployeeManager, Page.MachineManager, Page.FoodManager, Page.MenuManager, Page.OrderManager,
                Page.RoleManager, Page.ShiftManager, Page.StationCategoryManager, Page.StationManager,
                Page.RestaurantManager
            ],
            [DefaultRole.Customer] = [Page.OrderCreator],
            [DefaultRole.Employee] = [Page.StationTasks, Page.OrderCreator, Page.OrderStatusDisplay],
            [DefaultRole.Machine] = [Page.OrderStatusDisplay, Page.StationTasks, Page.OrderCreator]
        };

    public async Task InitializeAsync()
    {
        foreach (var defaultRole in Enum.GetValues<DefaultRole>())
        {
            var roleName = defaultRole.ToString();
            if (await roleManager.RoleExistsAsync(roleName))
                continue;

            var role = new Role
            {
                Name = roleName,
                IsDefault = true,
                IsImmutable = defaultRole == DefaultRole.Admin,
                Pages = DefaultRolePages.TryGetValue(defaultRole, out var pages) ? pages.ToList() : []
            };

            var roleResult = await roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
                throw new ValidationAppException(
                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));

            if (!DefaultRoles.TryGetValue(defaultRole, out var permissions))
                continue;

            foreach (var permission in permissions)
            {
                var claimResult = await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                if (!claimResult.Succeeded)
                    throw new ValidationAppException(
                        string.Join("; ", claimResult.Errors.Select(e => e.Description)));
            }
        }

        await InitializeAdminRoleAsync();
    }

    private async Task InitializeAdminRoleAsync()
    {
        var adminRoleName = nameof(DefaultRole.Admin);
        var adminRole = await roleManager.FindByNameAsync(adminRoleName);
        if (adminRole == null)
        {
            adminRole = new Role { Name = adminRoleName, IsDefault = true, IsImmutable = true };
            var createResult = await roleManager.CreateAsync(adminRole);
            if (!createResult.Succeeded)
                throw new ValidationAppException(
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
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