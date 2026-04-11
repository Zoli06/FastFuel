using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Employees.DTOs;

public record EmployeeRequestDto : UserRequestDto
{
    /// <summary>
    /// Email address of the employee.
    /// </summary>
    public required string Email { get; init; }
    /// <summary>
    /// The list of shift identifiers.
    /// </summary>
    public required List<uint> ShiftIds { get; init; }
    /// <summary>
    /// The list of station category identifiers.
    /// </summary>
    public required List<uint> StationCategoryIds { get; init; }
    /// <summary>
    /// The restaurant identifier where the employee works at.
    /// </summary>
    public required uint WorksAtRestaurantId { get; init; }
}