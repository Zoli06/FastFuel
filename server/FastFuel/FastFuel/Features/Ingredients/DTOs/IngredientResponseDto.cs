namespace FastFuel.Features.Ingredients.DTOs;

public class IngredientResponseDto
{
    public uint Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Uri? ImageUrl { get; set; }
    public List<uint> FoodIds { get; set; } = [];
    public List<uint> AllergyIds { get; set; } = [];
    public List<uint> StationCategoryIds { get; set; } = [];
}