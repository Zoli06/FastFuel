namespace FastFuel.Features.Foods.DTOs;

public class FoodRequestDto
{
    public string Name { get; set; } = string.Empty;
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public List<FoodIngredientDto> Ingredients { get; set; } = [];
}