using FastFuel.Features.Common.Services;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Shifts.DTOs;

namespace FastFuel.Features.Shifts.Services;

public interface IShiftService : ICrudService<ShiftRequestDto, ShiftResponseDto>
{
    public Task<List<ShiftResponseDto>> GetShiftsForCurrentEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);
}