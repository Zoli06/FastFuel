using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Admins.DTOs;

public record AdminRequestDto : UserRequestDto
{
    public required string Email { get; init; }
}