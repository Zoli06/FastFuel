namespace FastFuel.Features.Menus.DTOs;

public record MenuFoodDto
{
    /// <summary>
    /// The food identifier.
    /// </summary>
    public required uint FoodId { get; init; }
    /// <summary>
    /// The quantity of the food in the menu.
    /// </summary>
    public required uint Quantity { get; init; }
}