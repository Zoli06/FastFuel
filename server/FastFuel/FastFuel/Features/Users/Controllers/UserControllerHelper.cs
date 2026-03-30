using System.Security.Claims;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastFuel.Features.Users.Controllers;

public static class UserControllerHelper
{
    public static uint? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && uint.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }

    public static async Task<Results<Ok<TUserResponseDto>, NotFound, UnauthorizedHttpResult>>
        GetCurrentUser<TUserRequestDto, TUserResponseDto>(
            IUserService<TUserRequestDto, TUserResponseDto> userService,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default)
        where TUserRequestDto : UserRequestDto
        where TUserResponseDto : UserResponseDto
    {
        var userId = GetUserId(user);
        if (userId == null)
            return TypedResults.Unauthorized();

        var userDto = await userService.GetByIdAsync(userId.Value, userId, cancellationToken);
        if (userDto == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(userDto);
    }
}