namespace FastFuel.Features.Foods.DTOs;

public record FoodIngredientDto
{
    public required uint IngredientId { get; init; }
    public required uint Quantity { get; init; }
}