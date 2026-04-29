namespace FastFuel.Features.OrderFoods.DTOs;

public record OrderFoodResponseDto
{
    /// <summary>
    ///     The food identifier.
    /// </summary>
    public required uint? FoodId { get; init; }

    /// <summary>
    ///     The original food name at the time of order placement.
    /// </summary>
    public required string OriginalFoodName { get; init; }

    /// <summary>
    ///     The original food price at the time of order placement.
    /// </summary>
    public required double OriginalFoodPrice { get; init; }

    /// <summary>
    ///     The quantity of the food in the order.
    /// </summary>
    public required uint Quantity { get; init; }

    /// <summary>
    ///     Additional instructions for preparation.
    /// </summary>
    public required string? SpecialInstructions { get; init; }
}