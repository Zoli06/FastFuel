using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Models;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Users.Services;

public abstract class UserService<TUser, TUserRequestDto, TUserResponseDto>(
    ApplicationDbContext dbContext,
    IMapper<TUser, TUserRequestDto, TUserResponseDto> mapper,
    IPasswordHasher<User> passwordHasher)
    : CrudService<TUser, TUserRequestDto, TUserResponseDto>(dbContext, mapper)
    where TUser : User
    where TUserRequestDto : UserRequestDto
    where TUserResponseDto : UserResponseDto
{
    protected override Task OnBeforeCreateModelAsync(TUser model, TUserRequestDto requestDto)
    {
        if (requestDto.Password == null)
            throw new ArgumentException("Password is required for customer creation.");
        model.PasswordHash = passwordHasher.HashPassword(model, requestDto.Password);
        return Task.CompletedTask;
    }

    protected override Task OnBeforeUpdateModelAsync(TUser model, TUserRequestDto requestDto)
    {
        if (requestDto.Password != null) model.PasswordHash = passwordHasher.HashPassword(model, requestDto.Password);
        return Task.CompletedTask;
    }
}