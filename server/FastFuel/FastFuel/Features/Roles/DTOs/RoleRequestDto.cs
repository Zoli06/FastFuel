using FastFuel.Features.Pages.Common;

namespace FastFuel.Features.Roles.DTOs;

public record RoleRequestDto
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
}