namespace FastFuel.Features.Foods.DTOs;

public class EditFoodDto
{
    public required string Name { get; set; }
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }

    // Allow editing the list of ingredient ids when creating/updating a food
    public required List<uint> IngredientIds { get; set; } = new();
}