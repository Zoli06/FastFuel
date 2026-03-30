namespace FastFuel.Features.Ingredients.DTOs;

public record IngredientRequestDto
{
    public required string Name { get; init; }
    public required Uri? ImageUrl { get; init; }
    public required List<uint> AllergyIds { get; init; }
    public required List<uint> StationCategoryIds { get; init; }
    public required uint DefaultTimerValueSeconds { get; init; }
}