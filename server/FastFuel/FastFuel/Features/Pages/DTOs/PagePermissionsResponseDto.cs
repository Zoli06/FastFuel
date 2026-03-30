using FastFuel.Features.Pages.Common;
using FastFuel.Features.Roles.Common;

namespace FastFuel.Features.Pages.DTOs;

public record PagePermissionsResponseDto
{
    public required Page Page { get; init; }
    public required List<string> NecessaryPermissions { get; init; }
    public required List<string> RecommendedPermissions { get; init; }
    public required List<DefaultRole> RequiresDefaultRole { get; init; }
}