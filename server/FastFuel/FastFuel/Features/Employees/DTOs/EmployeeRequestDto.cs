namespace FastFuel.Features.Employees.DTOs;

public class EmployeeRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public uint ThemeId { get; init; }
    public string Password { get; set; } = string.Empty;
}