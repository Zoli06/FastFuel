namespace FastFuel.Features.Users.DTOs;

public record UserRequestDto
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The login username.
    /// </summary>
    public required string UserName { get; init; }
    /// <summary>
    /// The account password.
    /// </summary>
    public required string? Password { get; init; }
}