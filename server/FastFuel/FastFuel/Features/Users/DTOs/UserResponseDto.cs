using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Users.DTOs;

public record UserResponseDto : IIdentifiable
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
    /// The list of role identifiers.
    /// </summary>
    public required List<uint> RoleIds { get; init; }
    /// <summary>
    /// The user type.
    /// </summary>
    public required string UserType { get; init; }
    /// <summary>
    /// The list of order identifiers.
    /// </summary>
    public required List<uint> OrderIds { get; init; }
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}