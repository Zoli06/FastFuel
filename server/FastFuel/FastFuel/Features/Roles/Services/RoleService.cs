using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Pages.Common;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Roles.Services;

public class RoleService(
    FastFuelDbContext dbContext,
    IMapper<Role, RoleRequestDto, RoleResponseDto> mapper,
    RoleManager<Role> roleManager,
    UserManager<User> userManager)
    : CrudService<Role, RoleRequestDto, RoleResponseDto>(dbContext, mapper), IRoleService
{
    protected override DbSet<Role> DbSet => DbContext.Roles;

    protected override Create<Role, RoleRequestDto, RoleResponseDto> CreateOperation =>
        new Create(DbContext, DbSet, Mapper, roleManager, userManager);

    protected override Update<Role, RoleRequestDto, RoleResponseDto> UpdateOperation =>
        new Update(DbContext, DbSet, Mapper, roleManager, userManager);

    protected override Delete<Role> DeleteOperation =>
        new Delete(DbContext, DbSet);

    public async Task<List<RoleResponseDto>> GetRolesForCurrentUserAsync(ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new ResourceNotFoundAppException(nameof(ClaimsPrincipal), nameof(user));

        if (!uint.TryParse(userIdClaim.Value, out var userId))
            throw new ResourceNotFoundAppException(nameof(ClaimsPrincipal), nameof(userIdClaim));

        var appUser = await userManager.FindByIdAsync(userId.ToString());
        if (appUser == null)
            throw new ResourceNotFoundAppException(nameof(User), userId);

        var roleNames = await userManager.GetRolesAsync(appUser);
        if (roleNames.Count == 0)
            return [];

        var roles = await DbSet
            .Where(role => roleNames.Contains(role.Name))
            .ToListAsync(cancellationToken);

        return roles.ConvertAll(Mapper.ToDto);
    }

    private static async Task UpdateRoleClaimsAsync(RoleManager<Role> roleManager, Role role,
        List<string> newPermissions)
    {
        var existingClaims = await roleManager.GetClaimsAsync(role);
        var permissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();
        var existingPermissions = permissionClaims.Select(c => c.Value).ToHashSet();
        var newPermissionsSet = newPermissions.ToHashSet();

        var permissionsToRemove = existingPermissions.Except(newPermissionsSet).ToList();
        var permissionsToAdd = newPermissionsSet.Except(existingPermissions).ToList();

        var permissionsToRemoveSet = permissionsToRemove.ToHashSet();
        foreach (var claim in permissionClaims.Where(c => permissionsToRemoveSet.Contains(c.Value)))
            await roleManager.RemoveClaimAsync(role, claim);

        foreach (var permission in permissionsToAdd)
            await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
    }

    private static async Task UpdateRoleUsersAsync(UserManager<User> userManager, Role role,
        List<uint> newUserIds)
    {
        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
        var existingUserIds = usersInRole.Select(u => u.Id).ToHashSet();
        var newUserIdsSet = newUserIds.ToHashSet();

        var userIdsToRemove = existingUserIds.Except(newUserIdsSet).ToList();
        var userIdsToAdd = newUserIdsSet.Except(existingUserIds).ToList();

        foreach (var user in userIdsToRemove.Select(userId => usersInRole.FirstOrDefault(u => u.Id == userId))
                     .OfType<User>()) await userManager.RemoveFromRoleAsync(user, role.Name);

        foreach (var userId in userIdsToAdd)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null) await userManager.AddToRoleAsync(user, role.Name);
        }
    }

    private static async Task EnsureDefaultRoleUsersUnchangedAsync(UserManager<User> userManager, Role role,
        List<uint> requestedUserIds)
    {
        if (!role.IsDefault)
            return;

        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
        var existingUserIds = usersInRole.Select(u => u.Id).ToHashSet();

        if (!existingUserIds.SetEquals(requestedUserIds))
            throw new UnauthorizedAppException(
                $"Users of default role '{role.Name}' cannot be modified.");
    }

    private static async Task EnsurePermissionsUnchangedIfImmutableAsync(RoleManager<Role> roleManager, Role role,
        List<string> requestedPermissions)
    {
        if (!role.ArePermissionsImmutable)
            return;

        var currentPermissions = (await roleManager.GetClaimsAsync(role))
            .Where(claim => claim.Type == "Permission")
            .Select(claim => claim.Value)
            .ToHashSet();

        if (!currentPermissions.SetEquals(requestedPermissions))
            throw new UnauthorizedAppException($"Permissions of role '{role.Name}' cannot be modified.");
    }

    private static void EnsureAdminRoleHasRoleManagerPage(Role role, List<Page> requestedPages)
    {
        if (!role.IsDefault || !string.Equals(role.Name, nameof(DefaultRole.Admin), StringComparison.Ordinal))
            return;

        if (!requestedPages.Contains(Page.RoleManager))
            throw new UnauthorizedAppException(
                $"Page '{Page.RoleManager}' cannot be removed from role '{role.Name}'.");
    }

    private class Create(
        FastFuelDbContext dbContext,
        DbSet<Role> dbSet,
        IMapper<Role, RoleRequestDto, RoleResponseDto> mapper,
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
        : Create<Role, RoleRequestDto, RoleResponseDto>(dbContext, dbSet, mapper)
    {
        protected override async Task SaveEntityAsync(RoleRequestDto requestDto, Role entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            await base.SaveEntityAsync(requestDto, entity, userId, cancellationToken);

            await UpdateRoleClaimsAsync(roleManager, entity, requestDto.Permissions);
            await UpdateRoleUsersAsync(userManager, entity, requestDto.UserIds);
        }
    }

    private class Update(
        FastFuelDbContext dbContext,
        DbSet<Role> dbSet,
        IMapper<Role, RoleRequestDto, RoleResponseDto> mapper,
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
        : Update<Role, RoleRequestDto, RoleResponseDto>(dbContext, dbSet, mapper)
    {
        protected override Task UpdateEntityAsync(uint id, RoleRequestDto requestDto, Role entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            if (entity.IsDefault && !string.Equals(entity.Name, requestDto.Name, StringComparison.Ordinal))
                throw new UnauthorizedAppException($"The default role '{entity.Name}' cannot be renamed.");

            return base.UpdateEntityAsync(id, requestDto, entity, userId, cancellationToken);
        }

        protected override async Task SaveEntityAsync(
            uint id, RoleRequestDto requestDto,
            Role entity,
            uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            await EnsureDefaultRoleUsersUnchangedAsync(userManager, entity, requestDto.UserIds);
            await EnsurePermissionsUnchangedIfImmutableAsync(roleManager, entity, requestDto.Permissions);
            EnsureAdminRoleHasRoleManagerPage(entity, requestDto.Pages);

            await base.SaveEntityAsync(id, requestDto, entity, userId, cancellationToken);

            await UpdateRoleClaimsAsync(roleManager, entity, requestDto.Permissions);
            await UpdateRoleUsersAsync(userManager, entity, requestDto.UserIds);
        }
    }

    private class Delete(FastFuelDbContext dbContext, DbSet<Role> dbSet)
        : Delete<Role>(dbContext, dbSet)
    {
        protected override async Task DeleteEntityAsync(uint id, Role entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {

            if (entity.IsDefault)
                throw new UnauthorizedAppException(
                    $"The default role '{entity.Name}' cannot be deleted.");

            await base.DeleteEntityAsync(id, entity, userId, cancellationToken);
        }
    }
}