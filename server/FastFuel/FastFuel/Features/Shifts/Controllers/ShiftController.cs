using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Shifts.DTOs;
using FastFuel.Features.Shifts.Entities;

namespace FastFuel.Features.Shifts.Controllers;

public class ShiftController(ICrudService<ShiftRequestDto, ShiftResponseDto> service)
    : CrudController<Shift, ShiftRequestDto, ShiftResponseDto>(service);