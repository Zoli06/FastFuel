using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Users.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class UserController(
    IUserService<UserRequestDto, UserResponseDto> service,
    UserManager<User> userManager)
    : ControllerBase
{
    public IUserService<UserRequestDto, UserResponseDto> UserService { get; } = service;

    public UserManager<User> UserManager { get; } = userManager;

    [HttpGet("me")]
    public Task<Results<Ok<UserResponseDto>, NotFound, UnauthorizedHttpResult>>
        GetCurrentUser(CancellationToken cancellationToken = default)
    {
        return UserControllerHelper.GetCurrentUser(UserService, User, cancellationToken);
    }
}