using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Roles.Services;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Users.Services;

public abstract class UserServiceBase<TUser, TUserRequestDto, TUserResponseDto>(
    ApplicationDbContext dbContext,
    IMapper<TUser, TUserRequestDto, TUserResponseDto> mapper,
    UserManager<User> userManager)
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

    protected virtual DefaultRole[] DefaultRoles => [DefaultRole.User];

    private async Task SetDefaultRoles(TUser user)
    {
        foreach (var role in DefaultRoles)
        {
            var userResult = await userManager.AddToRoleAsync(user, role.ToRoleName());
            if (!userResult.Succeeded)
                throw new ValidationAppException(string.Join("; ", userResult.Errors.Select(e => e.Description)));
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
                throw new MissingRequiredFieldAppException("Password");

            var result = await userManager.CreateAsync(entity, requestDto.Password);
            if (!result.Succeeded) throw new ValidationAppException(string.Join("; ", result.Errors.Select(e => e.Description)));

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
                if (!result.Succeeded)
                    throw new ValidationAppException(string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            await base.SaveEntityAsync(id, requestDto, entity, userId, cancellationToken);
        }
    }
}