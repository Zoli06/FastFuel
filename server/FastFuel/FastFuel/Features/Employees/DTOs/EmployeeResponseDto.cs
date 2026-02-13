using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Employees.DTOs;

public class EmployeeResponseDto : IIdentifiable
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public uint ThemeId { get; init; }
    public uint Id { get; init; }
}