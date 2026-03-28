using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Users.Controllers;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Employees.Controllers;

public class EmployeeController(
    IUserService<EmployeeRequestDto, EmployeeResponseDto> service)
    : CrudController<Employee, EmployeeRequestDto, EmployeeResponseDto>(service)
{
    /// <summary>
    /// Gets the profile of the currently authenticated employee.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current employee profile when it exists.</returns>
    [HttpGet("me")]
    public Task<Results<Ok<EmployeeResponseDto>, NotFound, UnauthorizedHttpResult>> GetCurrentUser(
        CancellationToken cancellationToken = default)
    {
        return UserControllerHelper.GetCurrentUser(service, User, cancellationToken);
    }
}