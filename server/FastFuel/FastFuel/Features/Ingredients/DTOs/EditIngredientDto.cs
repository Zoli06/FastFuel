using FastFuel.Features.Foods.DTOs;

namespace FastFuel.Features.Ingredients.DTOs;

public class EditIngredientDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public required List<uint> AllergyIds { get; set; }
    public required List<uint> StationCategoryIds { get; set; }
}