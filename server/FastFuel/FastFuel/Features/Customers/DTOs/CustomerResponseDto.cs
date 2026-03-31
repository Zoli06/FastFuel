using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Customers.DTOs;

public record CustomerResponseDto : UserResponseDto
{
    public required string Email { get; init; }
}