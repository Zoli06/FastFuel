namespace FastFuel.Features.Shifts.DTOs;

public record ShiftRequestDto
{
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public required uint EmployeeId { get; init; }
}