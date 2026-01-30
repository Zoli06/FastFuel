using FastFuel.Features.Common;
using FastFuel.Features.Themes.DTOs;

namespace FastFuel.Features.Users.DTOs;

public class UserRequestDto
{
    public string Name { get; init; } = string.Empty;
    public ThemeRequestDto Theme { get; init; } = new ThemeRequestDto();
}