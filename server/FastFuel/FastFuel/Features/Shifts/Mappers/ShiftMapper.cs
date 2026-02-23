using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Shifts.DTOs;
using FastFuel.Features.Shifts.Models;

namespace FastFuel.Features.Shifts.Mappers;

public class ShiftMapper : IMapper<Shift, ShiftRequestDto, ShiftResponseDto>
{
    public ShiftResponseDto ToDto(Shift model)
    {
        return new ShiftResponseDto
        {
            Id = model.Id,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            EmployeeId = model.EmployeeId
        };
    }

    public Shift ToModel(ShiftRequestDto dto)
    {
        return new Shift
        {
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            EmployeeId = dto.EmployeeId
        };
    }

    public void UpdateModel(ShiftRequestDto dto, Shift model)
    {
        model.StartTime = dto.StartTime;
        model.EndTime = dto.EndTime;
        model.EmployeeId = dto.EmployeeId;
    }
}