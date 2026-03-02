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

    protected uint? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && uint.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }

    async Task<Results<
            Ok<TUserResponseDto>,
            NotFound,
            UnauthorizedHttpResult>>
        GetCurrentUserDefault(CancellationToken cancellationToken = default)
    {
        var userId = GetUserId(User);
        if (userId == null)
            return TypedResults.Unauthorized();

        var userDto = await UserService.GetByIdAsync(userId.Value, userId, cancellationToken);
        if (userDto == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(userDto);
    }

    Task<Results<
            Ok<TUserResponseDto>,
            NotFound,
            UnauthorizedHttpResult>>
        // ReSharper disable once UnusedMemberInSuper.Global
        GetCurrentUser(CancellationToken cancellationToken = default);
}