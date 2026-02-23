using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Shifts.DTOs;
using FastFuel.Features.Shifts.Entities;

namespace FastFuel.Features.Shifts.Mappers;

public class ShiftMapper : IMapper<Shift, ShiftRequestDto, ShiftResponseDto>
{
    public ShiftResponseDto ToDto(Shift entity)
    {
        return new ShiftResponseDto
        {
            Id = entity.Id,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            EmployeeId = entity.EmployeeId
        };
    }

    public Shift ToEntity(ShiftRequestDto dto)
    {
        return new Shift
        {
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            EmployeeId = dto.EmployeeId
        };
    }

    public void UpdateEntity(ShiftRequestDto dto, Shift entity)
    {
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.EmployeeId = dto.EmployeeId;
    }
}