using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Shifts.DTOs;
using FastFuel.Features.Shifts.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Shifts.Services;

public class ShiftService(ApplicationDbContext dbContext, IMapper<Shift, ShiftRequestDto, ShiftResponseDto> mapper)
    : CrudService<Shift, ShiftRequestDto, ShiftResponseDto>(dbContext, mapper)
{
    protected override DbSet<Shift> DbSet { get; } = dbContext.Shifts;
}