using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Users.DTOs;

public record UserResponseDto : IIdentifiable
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string UserName { get; init; }
    public required uint ThemeId { get; init; }
    public required uint Id { get; init; }
}