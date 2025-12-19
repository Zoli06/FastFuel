namespace FastFuel.Features.Orders.DTOs;

public class PlaceOrderDto
{
    public uint RestaurantId { get; set; }
    public List<OrderMenuDto> Menus { get; set; } = [];
    public List<OrderFoodDto> Foods { get; set; } = [];
}