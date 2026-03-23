using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Admins.DTOs;

public record AdminResponseDto : UserResponseDto
{
    public required string Email { get; init; }
}