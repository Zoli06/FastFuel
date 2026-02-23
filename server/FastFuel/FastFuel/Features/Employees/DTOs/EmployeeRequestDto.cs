using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Employees.DTOs;

public record EmployeeRequestDto : UserRequestDto
{
    public required List<uint> ShiftIds { get; init; }
    public required List<uint> StationCategoryIds { get; init; }
}