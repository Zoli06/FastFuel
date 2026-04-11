using FastFuel.Features.Pages.Common;
using FastFuel.Features.Roles.Common;

namespace FastFuel.Features.Pages.DTOs;

public record PagePermissionsResponseDto
{
    /// <summary>
    /// The name of the page.
    /// </summary>
    public required Page Page { get; init; }
    /// <summary>
    /// The list of necessary permissions.
    /// </summary>
    public required List<string> NecessaryPermissions { get; init; }
    /// <summary>
    /// The list of recommended permissions.
    /// </summary>
    public required List<string> RecommendedPermissions { get; init; }
    /// <summary>
    /// The list of required default roles.
    /// </summary>
    public required List<DefaultRole> RequiresDefaultRole { get; init; }
    /// <summary>
    /// The list of required for default role.
    /// </summary>
    public required List<DefaultRole> RequiredForDefaultRole { get; init; }
}