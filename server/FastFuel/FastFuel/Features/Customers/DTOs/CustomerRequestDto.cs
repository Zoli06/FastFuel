using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Customers.DTOs;

public record CustomerRequestDto : UserRequestDto
{
    /// <summary>
    /// Email address of the customer.
    /// </summary>
    public required string Email { get; init; }
}