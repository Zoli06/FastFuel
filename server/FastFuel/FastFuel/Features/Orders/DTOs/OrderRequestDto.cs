namespace FastFuel.Features.Orders.DTOs;

public class OrderRequestDto
{
    public uint CustomerId { get; init; } //TODO: check if the logined CustomerId is same as the one we got
    public uint RestaurantId { get; init; }
    public List<OrderMenuDto> Menus { get; init; } = [];
    public List<OrderFoodDto> Foods { get; init; } = [];
}