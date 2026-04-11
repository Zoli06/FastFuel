namespace FastFuel.Features.Orders.DTOs;

public record OrderFoodDto
{
    /// <summary>
    /// The food identifier.
    /// </summary>
    public required uint FoodId { get; init; }
    /// <summary>
    /// The quantity of the food in the order.
    /// </summary>
    public required uint Quantity { get; init; }
    /// <summary>
    /// Additional instructions for preparation.
    /// </summary>
    public required string? SpecialInstructions { get; init; }
}