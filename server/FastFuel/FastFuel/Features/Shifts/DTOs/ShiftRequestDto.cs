namespace FastFuel.Features.Shifts.DTOs;

public record ShiftRequestDto
{
    /// <summary>
    /// The shift start date and time.
    /// </summary>
    public required DateTime StartTime { get; init; }
    /// <summary>
    /// The shift end date and time.
    /// </summary>
    public required DateTime EndTime { get; init; }
    /// <summary>
    /// The employee identifier.
    /// </summary>
    public required uint EmployeeId { get; init; }
}