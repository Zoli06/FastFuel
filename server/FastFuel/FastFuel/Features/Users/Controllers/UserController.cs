using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Users.Controllers;

[Authorize]
[ApiController]
[Route("/api/[controller]")]
public class UserController(
    IUserService<UserRequestDto, UserResponseDto> service)
    : ControllerBase
{
    protected IUserService<UserRequestDto, UserResponseDto> UserService { get; } = service;

    [HttpGet]
    [PermissionCheck(CrudOperation.Read)]
    public async Task<Results<
            Ok<List<UserResponseDto>>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        GetAll(CancellationToken cancellationToken = default)
    {
        var dtos = await UserService.GetAllAsync(UserControllerHelper.GetUserId(User), cancellationToken);
        return TypedResults.Ok(dtos);
    }

    [HttpGet("{id:int}")]
    [PermissionCheck(CrudOperation.Read)]
    public async Task<Results<
            Ok<UserResponseDto>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        GetById(uint id, CancellationToken cancellationToken = default)
    {
        var dto = await UserService.GetByIdAsync(id, UserControllerHelper.GetUserId(User), cancellationToken);
        if (dto == null)
            return TypedResults.NotFound();
        return TypedResults.Ok(dto);
    }

    [HttpGet("me")]
    public Task<Results<Ok<UserResponseDto>, NotFound, UnauthorizedHttpResult>>
        GetCurrentUser(CancellationToken cancellationToken = default)
    {
        return UserControllerHelper.GetCurrentUser(UserService, User, cancellationToken);
    }
}