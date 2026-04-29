using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Pages.Common;

namespace FastFuel.Features.Roles.DTOs;

public record RoleResponseDto : IIdentifiable
{
    /// <summary>
    /// The displayed name.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The list of permissions.
    /// </summary>
    public required List<string> Permissions { get; init; }
    /// <summary>
    /// The list of pages.
    /// </summary>
    public required List<Page> Pages { get; init; }
    /// <summary>
    /// The list of user identifiers.
    /// </summary>
    public required List<uint> UserIds { get; init; }
    /// <summary>
    /// Indicates whether this role is the default role.
    /// </summary>
    public required bool IsDefault { get; init; }
    /// <summary>
    /// Indicates whether role permissions are immutable.
    /// </summary>
    public required bool ArePermissionsImmutable { get; init; }
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}