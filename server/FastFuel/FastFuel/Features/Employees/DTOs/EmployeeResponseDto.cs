using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Employees.DTOs;

public record EmployeeResponseDto : UserResponseDto
{
    public required List<uint> ShiftIds { get; init; }
    public required List<uint> StationCategoryIds { get; init; }
}