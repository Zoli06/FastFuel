namespace FastFuel.Features.Foods.DTOs;

public record FoodRequestDto
{
    public required string Name { get; init; }
    public required uint Price { get; init; }
    public required string? Description { get; init; }
    public required Uri? ImageUrl { get; init; }
    public required List<FoodIngredientDto> Ingredients { get; init; }
}