using FastFuel.Features.Users.DTOs;

namespace FastFuel.Features.Machines.DTOs;

public record MachineResponseDto : UserResponseDto
{
    /// <summary>
    /// The restaurant identifier where the machine is located at.
    /// </summary>
    public required uint LocatedAtRestaurantId { get; init; }
}