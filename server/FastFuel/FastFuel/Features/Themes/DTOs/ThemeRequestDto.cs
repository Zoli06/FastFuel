namespace FastFuel.Features.Themes.DTOs;

public class ThemeRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string Background { get; init; } = "#ffffff";
    public string Footer { get; init; } = "#f0f0f0";
    public string ButtonPrimary { get; init; } = "#007bff";
    public string ButtonSecondary { get; init; } = "#6c757d";
}