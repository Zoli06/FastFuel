using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Customers.DTOs;

public record CustomerRequestDto : UserRequestDto
{
    public required string Email { get; init; }
}