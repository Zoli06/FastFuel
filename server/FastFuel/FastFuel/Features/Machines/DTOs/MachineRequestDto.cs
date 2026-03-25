using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Machines.DTOs;

public record MachineRequestDto : UserRequestDto
{
    public required uint LocatedAtRestaurantId { get; init; }
}