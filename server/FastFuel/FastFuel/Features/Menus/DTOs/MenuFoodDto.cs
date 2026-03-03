namespace FastFuel.Features.Menus.DTOs;

public record MenuFoodDto
{
    public required uint FoodId { get; init; }
    public required uint Quantity { get; init; }
}