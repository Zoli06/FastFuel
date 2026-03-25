using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.Common;

namespace FastFuel.Features.Roles.DTOs;

public record RoleResponseDto : IIdentifiable
{
    public required string Name { get; init; }
    public required List<string> Permissions { get; init; }
    public required List<Page> Pages { get; init; }
    public required List<uint> UserIds { get; init; }
    public required bool IsDefault { get; init; }
    public required bool IsImmutable { get; init; }
    public required uint Id { get; init; }
}