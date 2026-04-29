using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Shifts.DTOs;

public record ShiftResponseDto : IIdentifiable
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
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}