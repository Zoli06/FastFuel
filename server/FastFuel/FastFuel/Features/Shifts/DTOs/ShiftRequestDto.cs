namespace FastFuel.Features.Shifts.DTOs;

public class ShiftRequestDto
{
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
    public uint EmployeeId { get; init; }
}