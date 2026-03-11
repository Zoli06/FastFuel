using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Users.Controllers;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Employees.Controllers;

public class EmployeeController(
    IUserService<EmployeeRequestDto, EmployeeResponseDto> service,
    UserManager<User> userManager)
    : CrudController<Employee, EmployeeRequestDto, EmployeeResponseDto>(service)
{
    public IUserService<EmployeeRequestDto, EmployeeResponseDto> UserService { get; } = service;
    public UserManager<User> UserManager { get; } = userManager;

    [HttpGet("me")]
    public Task<Results<Ok<EmployeeResponseDto>, NotFound, UnauthorizedHttpResult>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        return UserControllerHelper.GetCurrentUser(UserService, User, cancellationToken);
    }
}