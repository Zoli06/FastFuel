using FastFuel.Features.Roles.Common;

namespace FastFuel.Features.Roles.DTOs;

public record RoleRequestDto
{
    public required string Name { get; init; }
    public required List<string> Permissions { get; init; }
    public required List<Page> Pages { get; init; }
    public required List<uint> UserIds { get; init; }
}