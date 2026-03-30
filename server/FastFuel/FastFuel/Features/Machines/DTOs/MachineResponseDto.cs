using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Machines.DTOs;

public record MachineResponseDto : UserResponseDto
{
    public required uint LocatedAtRestaurantId { get; init; }
}