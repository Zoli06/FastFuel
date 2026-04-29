using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Shifts.DTOs;
using FastFuel.Features.Shifts.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Shifts.Services;

public class ShiftService(FastFuelDbContext dbContext, IMapper<Shift, ShiftRequestDto, ShiftResponseDto> mapper)
    : CrudService<Shift, ShiftRequestDto, ShiftResponseDto>(dbContext, mapper), IShiftService
{
    protected override DbSet<Shift> DbSet { get; } = dbContext.Shifts;

    public async Task<List<ShiftResponseDto>> GetShiftsForCurrentEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        var shifts = await DbSet
            .Where(shift => shift.EmployeeId == employee.Id)
            .ToListAsync(cancellationToken);

        return shifts.ConvertAll(Mapper.ToDto);
    }
}