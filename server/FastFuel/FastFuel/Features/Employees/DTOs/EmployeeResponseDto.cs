using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Employees.DTOs;

public record EmployeeResponseDto : UserResponseDto
{
    public required string Email { get; init; }
    public required List<uint> ShiftIds { get; init; }
    public required List<uint> StationCategoryIds { get; init; }
    public required uint WorksAtRestaurantId { get; init; }
}