using System.Security.Claims;
using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Common.Permissions;
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
    [HttpGet("my")]
    [PermissionCheck("ReadOwn")]
    public async Task<ActionResult<List<ShiftResponseDto>>> GetShiftsForCurrentEmployee(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var employee = await userManager.GetUserAsync(user) as Employee;
        if (employee == null)
        {
            throw new ResourceNotFoundAppException(nameof(Employee), userManager.GetUserId(user) ?? "");
        }

        return await service.GetShiftsForCurrentEmployeeAsync(employee, cancellationToken);
    }
}