namespace FastFuel.Features.Themes.DTOs;

public record ThemeRequestDto
{
    public required string Name { get; init; }
    public required string Background { get; init; }
    public required string Footer { get; init; }
    public required string ButtonPrimary { get; init; }
    public required string ButtonSecondary { get; init; }
}