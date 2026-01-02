namespace FastFuel.Features.Foods.DTOs;

public class FoodRequestDto
{
    public string Name { get; init; } = string.Empty;
    public uint Price { get; init; }
    public string? Description { get; init; }
    public Uri? ImageUrl { get; init; }
    public List<FoodIngredientDto> Ingredients { get; init; } = [];
}