using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Ingredients.DTOs;

public record IngredientResponseDto : IIdentifiable
{
    public required string Name { get; init; }
    public required Uri? ImageUrl { get; init; }
    public required List<uint> FoodIds { get; init; }
    public required List<uint> AllergyIds { get; init; }
    public required List<uint> StationCategoryIds { get; init; }
    public required TimeSpan DefaultTimerValue { get; init; }
    public required uint Id { get; init; }
}