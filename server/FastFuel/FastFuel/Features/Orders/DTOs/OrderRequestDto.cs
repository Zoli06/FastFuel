namespace FastFuel.Features.Orders.DTOs;

public record OrderRequestDto
{
    public required uint RestaurantId { get; init; }
    public required List<OrderMenuDto> Menus { get; init; }
    public required List<OrderFoodDto> Foods { get; init; }
}