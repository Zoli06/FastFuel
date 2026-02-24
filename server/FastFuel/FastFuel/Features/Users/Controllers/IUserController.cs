using System.Security.Claims;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Users.Controllers;

public interface IUserController<TUserRequestDto, TUserResponseDto>
    where TUserRequestDto : UserRequestDto
    where TUserResponseDto : UserResponseDto
{
    protected IUserService<TUserRequestDto, TUserResponseDto> UserService { get; }
    protected UserManager<User> UserManager { get; }
    protected ClaimsPrincipal User { get; }

    async Task<Results<
            Ok<TUserResponseDto>,
            UnauthorizedHttpResult>>
        GetCurrentUserDefault(CancellationToken cancellationToken = default)
    {
        var user = await UserManager.GetUserAsync(User);
        if (user == null)
            return TypedResults.Unauthorized();

        var userResponseDto = await UserService.GetByIdAsync(user.Id, cancellationToken);
        return TypedResults.Ok(userResponseDto!);
    }

    Task<Results<
            Ok<TUserResponseDto>,
            UnauthorizedHttpResult>>
        // ReSharper disable once UnusedMemberInSuper.Global
        GetCurrentUser(CancellationToken cancellationToken = default);
}