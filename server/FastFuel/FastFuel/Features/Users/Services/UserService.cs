using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Users.Services;

public abstract class UserService<TUser, TUserRequestDto, TUserResponseDto>(
    ApplicationDbContext dbContext,
    IMapper<TUser, TUserRequestDto, TUserResponseDto> mapper,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
    : CrudService<TUser, TUserRequestDto, TUserResponseDto>(dbContext, mapper),
        IUserService<TUserRequestDto, TUserResponseDto>
    where TUser : User
    where TUserRequestDto : UserRequestDto
    where TUserResponseDto : UserResponseDto
{
    protected override Create<TUser, TUserRequestDto, TUserResponseDto> CreateOperation =>
        new Create(DbContext, DbSet, Mapper, userManager, SetDefaultRoles);

    protected override Update<TUser, TUserRequestDto, TUserResponseDto> UpdateOperation =>
        new Update(DbContext, DbSet, Mapper, userManager);

    protected virtual List<(string RoleName, string[] Permissions)> DefaultRoles =>
    [
        ("User",
        [
            "Permission:Menu:Read",
            "Permission:Food:Read",
            "Permission:Ingredient:Read",
            "Permission:Allergy:Read",
            "Permission:Restaurant:Read"
        ])
    ];

    private async Task SetDefaultRoles(TUser user)
    {
        foreach (var (roleName, permissions) in DefaultRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role { Name = roleName };
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                    throw new Exception(string.Join("; ", roleResult.Errors.Select(e => e.Description)));

                foreach (var permission in permissions)
                {
                    var claimResult = await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    if (!claimResult.Succeeded)
                        throw new Exception(string.Join("; ", claimResult.Errors.Select(e => e.Description)));
                }
            }

            var userResult = await userManager.AddToRoleAsync(user, roleName);
            if (!userResult.Succeeded)
                throw new Exception(string.Join("; ", userResult.Errors.Select(e => e.Description)));
        }
    }

    private class Create(
        ApplicationDbContext dbContext,
        DbSet<TUser> dbSet,
        IMapper<TUser, TUserRequestDto, TUserResponseDto> mapper,
        UserManager<User> userManager,
        Func<TUser, Task> setDefaultRoles
    ) : Create<TUser, TUserRequestDto, TUserResponseDto>(dbContext, dbSet, mapper)
    {
        protected override async Task SaveEntityAsync(TUserRequestDto requestDto, TUser entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            if (requestDto.Password is null)
                throw new ArgumentException("Password is required for user creation.");

            var result = await userManager.CreateAsync(entity, requestDto.Password);
            if (!result.Succeeded) throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await setDefaultRoles(entity);
        }
    }

    private class Update(
        ApplicationDbContext dbContext,
        DbSet<TUser> dbSet,
        IMapper<TUser, TUserRequestDto, TUserResponseDto> mapper,
        UserManager<User> userManager
    ) : Update<TUser, TUserRequestDto, TUserResponseDto>(dbContext, dbSet, mapper)
    {
        protected override async Task SaveEntityAsync(
            uint id,
            TUserRequestDto requestDto,
            TUser entity,
            uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            if (requestDto.Password != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(entity);
                var result = await userManager.ResetPasswordAsync(entity, token, requestDto.Password);
                if (!result.Succeeded) throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
            else
            {
                DbContext.Entry(entity).State = EntityState.Modified;
                await DbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}

public class UserService(
    ApplicationDbContext dbContext,
    IMapper<User, UserRequestDto, UserResponseDto> mapper,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
    : UserService<User, UserRequestDto, UserResponseDto>(dbContext, mapper, userManager, roleManager)
{
    protected override DbSet<User> DbSet { get; } = dbContext.Users;
}