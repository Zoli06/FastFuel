namespace FastFuel.Features.Customers.DTOs;

public class CustomerRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public uint ThemeId { get; init; }
    public string Password { get; set; } = string.Empty;
}