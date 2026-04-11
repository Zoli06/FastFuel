namespace FastFuel.Features.Foods.DTOs;

public record FoodIngredientDto
{
    /// <summary>
    /// The ingredient identifier.
    /// </summary>
    public required uint IngredientId { get; init; }
    /// <summary>
    /// The quantity of the food.
    /// </summary>
    public required uint Quantity { get; init; }
    /// <summary>
    /// The unit of measurement for the quantity.
    /// </summary>
    public required string Unit { get; init; }
}