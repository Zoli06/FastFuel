namespace FastFuel.Features.Orders.DTOs;

public class OrderRequestDto
{
    public uint RestaurantId { get; init; }
    public List<OrderMenuDto> Menus { get; init; } = [];
    public List<OrderFoodDto> Foods { get; init; } = [];
}