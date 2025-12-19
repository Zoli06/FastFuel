namespace FastFuel.Features.Ingredients.DTOs;

public class IngredientDto
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public required List<uint> FoodIds { get; set; }
    public required List<uint> AllergyIds { get; set; }
    public required List<uint> StationCategoryIds { get; set; }
}