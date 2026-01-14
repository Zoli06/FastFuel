namespace FastFuel.Features.Ingredients.DTOs;

public class IngredientRequestDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Uri? ImageUrl { get; init; }
    public List<uint> AllergyIds { get; init; } = [];
    public List<uint> StationCategoryIds { get; init; } = [];
    public TimeSpan DefaultTimerValue { get; init; }
}