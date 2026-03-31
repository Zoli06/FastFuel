using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Shifts.DTOs;
using FastFuel.Features.Shifts.Entities;
using FastFuel.Features.Shifts.Services;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Shifts.Controllers;

public class ShiftController(IShiftService service, UserManager<User> userManager)
    : CrudController<Shift, ShiftRequestDto, ShiftResponseDto>(service)
{
    /// <summary>
    /// Gets the shifts of the currently authenticated employee.
    /// </summary>
    /// <param name="user">The current authenticated user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current employee's shifts.</returns>
    [HttpGet("my")]
    public async Task<ActionResult<List<ShiftResponseDto>>> GetShiftsForCurrentEmployee(CancellationToken cancellationToken = default)
    {
        var employee = await userManager.GetUserAsync(User) as Employee;
        if (employee == null)
        {
            throw new ResourceNotFoundAppException(nameof(Employee), userManager.GetUserId(User) ?? "");
        }

        return await service.GetShiftsForCurrentEmployeeAsync(employee, cancellationToken);
    }
}