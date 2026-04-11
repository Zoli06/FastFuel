namespace FastFuel.Features.Auth.DTOs;

public record LoginRequestDto
{
    /// <summary>
    /// The login username.
    /// </summary>
    public required string UserName { get; init; }
    /// <summary>
    /// The account password.
    /// </summary>
    public required string Password { get; init; }
}