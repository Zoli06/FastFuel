namespace FastFuel.Features.Ingredients.DTOs;

public class IngredientRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public List<uint> AllergyIds { get; set; } = [];
    public List<uint> StationCategoryIds { get; set; } = [];
}