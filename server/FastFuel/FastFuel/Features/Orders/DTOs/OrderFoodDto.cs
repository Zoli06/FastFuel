namespace FastFuel.Features.Orders.DTOs;

public class OrderFoodDto
{
    public uint FoodId { get; init; }
    public uint Quantity { get; init; }
    public string? SpecialInstructions { get; init; }
}