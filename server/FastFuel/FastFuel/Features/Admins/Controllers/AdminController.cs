using FastFuel.Features.Admins.DTOs;
using FastFuel.Features.Admins.Entities;
using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Users.Controllers;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Admins.Controllers;

public class AdminController(
    IUserService<AdminRequestDto, AdminResponseDto> service)
    : CrudController<Admin, AdminRequestDto, AdminResponseDto>(service)
{
    public IUserService<AdminRequestDto, AdminResponseDto> UserService { get; } = service;

    [HttpGet("me")]
    public Task<Results<Ok<AdminResponseDto>, NotFound, UnauthorizedHttpResult>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        return UserControllerHelper.GetCurrentUser(UserService, User, cancellationToken);
    }
}