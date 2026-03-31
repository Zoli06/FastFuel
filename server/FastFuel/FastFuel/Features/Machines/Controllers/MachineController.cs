using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Machines.DTOs;
using FastFuel.Features.Machines.Entities;
using FastFuel.Features.Users.Controllers;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Machines.Controllers;

public class MachineController(
    IUserService<MachineRequestDto, MachineResponseDto> service)
    : CrudController<Machine, MachineRequestDto, MachineResponseDto>(service)
{
    public IUserService<MachineRequestDto, MachineResponseDto> UserService { get; } = service;

    /// <summary>
    /// Gets the profile of the currently authenticated machine user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current machine profile when it exists.</returns>
    [HttpGet("me")]
    public Task<Results<Ok<MachineResponseDto>, NotFound, UnauthorizedHttpResult>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        return UserControllerHelper.GetCurrentUser(UserService, User, cancellationToken);
    }
}