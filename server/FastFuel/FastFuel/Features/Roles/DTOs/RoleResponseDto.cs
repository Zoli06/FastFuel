using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Roles.DTOs;

public record RoleResponseDto : IIdentifiable
{
    public required string Name { get; init; }
    public required List<string> Permissions { get; init; }
    public required List<uint> UserIds { get; init; }
    public required uint Id { get; init; }
}