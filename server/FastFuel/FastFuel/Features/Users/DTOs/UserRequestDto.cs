namespace FastFuel.Features.Users.DTOs;

public record UserRequestDto
{
    public required string Name { get; init; }
    public required string UserName { get; init; }
    public required string? Password { get; init; }
}