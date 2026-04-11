using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Admins.DTOs;

public record AdminResponseDto : UserResponseDto
{
    /// <summary>
    /// Email address of the admin.
    /// </summary>
    public required string Email { get; init; }
}