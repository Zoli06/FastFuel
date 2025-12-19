namespace FastFuel.Features.Orders.DTOs;

public class OrderFoodDto
{
    public uint FoodId { get; set; }
    public uint Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}