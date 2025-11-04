namespace FastFuel.Features.Foods.DTOs;

public class FoodDto
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    // TODO: Add Ingredients and Menus maybe with quantities
}