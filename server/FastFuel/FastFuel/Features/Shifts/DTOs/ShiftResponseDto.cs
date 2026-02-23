using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Shifts.DTOs;

public record ShiftResponseDto : IIdentifiable
{
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required uint EmployeeId { get; init; }
    public required uint Id { get; init; }
}