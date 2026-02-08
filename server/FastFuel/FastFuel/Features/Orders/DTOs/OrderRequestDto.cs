namespace FastFuel.Features.Orders.DTOs;

public class OrderRequestDto
{
    public uint UserId { get; init; } //TODO: check if the logined UserId is same as the one we got
    public uint RestaurantId { get; init; }
    public List<OrderMenuDto> Menus { get; init; } = [];
    public List<OrderFoodDto> Foods { get; init; } = [];
}