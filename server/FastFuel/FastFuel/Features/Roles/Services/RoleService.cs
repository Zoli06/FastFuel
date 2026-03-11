using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Roles.Services;

public class RoleService(
    ApplicationDbContext dbContext,
    IMapper<Role, RoleRequestDto, RoleResponseDto> mapper,
    RoleManager<Role> roleManager,
    UserManager<User> userManager)
    : CrudService<Role, RoleRequestDto, RoleResponseDto>(dbContext, mapper)
{
    protected override DbSet<Role> DbSet => DbContext.Roles;

    protected override Create<Role, RoleRequestDto, RoleResponseDto> CreateOperation =>
        new Create(DbContext, DbSet, Mapper, roleManager, userManager);

    protected override Update<Role, RoleRequestDto, RoleResponseDto> UpdateOperation =>
        new Update(DbContext, DbSet, Mapper, roleManager, userManager);

    protected override Delete<Role> DeleteOperation =>
        new Delete(DbContext, DbSet);

    private static async Task UpdateRoleClaimsAsync(RoleManager<Role> roleManager, Role role,
        List<string> newPermissions)
    {
        var existingClaims = await roleManager.GetClaimsAsync(role);
        var existingPermissions =
            existingClaims.Where(c => c.Type == "Permission").Select(c => c.Value).ToHashSet();
        var newPermissionsSet = newPermissions.ToHashSet();

        var permissionsToRemove = existingPermissions.Except(newPermissionsSet).ToList();
        var permissionsToAdd = newPermissionsSet.Except(existingPermissions).ToList();

        var claimsByValue = existingClaims.Where(c => c.Type == "Permission")
            .ToDictionary(c => c.Value);

        foreach (var permission in permissionsToRemove)
            if (claimsByValue.TryGetValue(permission, out var claim))
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

    private class Create(
        ApplicationDbContext dbContext,
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
        ApplicationDbContext dbContext,
        DbSet<Role> dbSet,
        IMapper<Role, RoleRequestDto, RoleResponseDto> mapper,
        RoleManager<Role> roleManager,
        UserManager<User> userManager)
        : Update<Role, RoleRequestDto, RoleResponseDto>(dbContext, dbSet, mapper)
    {
        protected override async Task SaveEntityAsync(
            uint id, RoleRequestDto requestDto,
            Role entity,
            uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            if (entity.IsDefault)
                throw new UnauthorizedAppException(
                    $"The default role '{entity.Name}' is immutable and its permissions cannot be modified.");

            await base.SaveEntityAsync(id, requestDto, entity, userId, cancellationToken);

            await UpdateRoleClaimsAsync(roleManager, entity, requestDto.Permissions);
            await UpdateRoleUsersAsync(userManager, entity, requestDto.UserIds);
        }
    }

    private class Delete(ApplicationDbContext dbContext, DbSet<Role> dbSet)
        : Delete<Role>(dbContext, dbSet)
    {
        protected override async Task DeleteEntityAsync(uint id, Role entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            if (entity.IsDefault)
                throw new UnauthorizedAppException(
                    $"The default role '{entity.Name}' is immutable and cannot be deleted.");

            await base.DeleteEntityAsync(id, entity, userId, cancellationToken);
        }
    }
}