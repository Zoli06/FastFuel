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
    : CrudController<Employee, EmployeeRequestDto, EmployeeResponseDto>(service),
        IUserController<EmployeeRequestDto, EmployeeResponseDto>
{
    public IUserService<EmployeeRequestDto, EmployeeResponseDto> Service { get; } = service;
    public UserManager<User> UserManager { get; } = userManager;

    [HttpGet("me")]
    public async Task<Results<Ok<EmployeeResponseDto>, UnauthorizedHttpResult>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        return await ((IUserController<EmployeeRequestDto, EmployeeResponseDto>)this).GetCurrentUserDefault(
            cancellationToken);
    }
}