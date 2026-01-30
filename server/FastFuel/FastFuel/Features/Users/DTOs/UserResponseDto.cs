using FastFuel.Features.Common;
using FastFuel.Features.Themes.DTOs;

namespace FastFuel.Features.Users.DTOs;

public class UserResponseDto : IIdentifiable
{
    public uint Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public ThemeResponseDto Theme { get; init; } = new ThemeResponseDto();
}