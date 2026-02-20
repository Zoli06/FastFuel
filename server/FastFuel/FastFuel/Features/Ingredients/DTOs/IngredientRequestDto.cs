namespace FastFuel.Features.Ingredients.DTOs;

public record IngredientRequestDto
{
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required Uri? ImageUrl { get; init; }
    public required List<uint> AllergyIds { get; init; }
    public required List<uint> StationCategoryIds { get; init; }
    public required TimeSpan DefaultTimerValue { get; init; }
}