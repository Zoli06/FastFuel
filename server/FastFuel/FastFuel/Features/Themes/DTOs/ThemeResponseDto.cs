using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Themes.DTOs;

public record ThemeResponseDto : IIdentifiable
{
    public required string Name { get; init; }
    public required string Background { get; init; }
    public required string Footer { get; init; }
    public required string ButtonPrimary { get; init; }
    public required string ButtonSecondary { get; init; }
    public required uint Id { get; init; }
}