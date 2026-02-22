using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Shifts.DTOs;

public class ShiftResponseDto : IIdentifiable
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public uint EmployeeId { get; init; }
    public uint Id { get; init; }
}