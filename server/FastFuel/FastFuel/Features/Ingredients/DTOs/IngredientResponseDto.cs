using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Ingredients.DTOs;

public class IngredientResponseDto : IIdentifiable
{
    public string Name { get; init; } = string.Empty;
    public Uri? ImageUrl { get; init; }
    public List<uint> FoodIds { get; init; } = [];
    public List<uint> AllergyIds { get; init; } = [];
    public List<uint> StationCategoryIds { get; init; } = [];
    public TimeSpan DefaultTimerValue { get; init; }
    public uint Id { get; init; }
}