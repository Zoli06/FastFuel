namespace FastFuel.Features.Orders.DTOs;

public record OrderFoodDto
{
    public required uint FoodId { get; init; }
    public required uint Quantity { get; init; }
    public required string? SpecialInstructions { get; init; }
}