namespace FastFuel.Features.Auth.DTOs;

public record LoginRequestDto
{
    public required string UserName { get; init; }
    public required string Password { get; init; }
}