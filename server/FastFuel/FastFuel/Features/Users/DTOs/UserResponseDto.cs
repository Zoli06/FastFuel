using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Users.DTOs;

public class UserResponseDto : IIdentifiable
{
    public uint Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public uint ThemeId { get; init; }
}