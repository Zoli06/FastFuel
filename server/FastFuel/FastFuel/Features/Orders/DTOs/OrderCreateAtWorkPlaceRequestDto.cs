namespace FastFuel.Features.Orders.DTOs;

public class OrderCreateAtWorkPlaceRequestDto
{
    public required List<OrderMenuDto> Menus { get; init; }
    public required List<OrderFoodDto> Foods { get; init; }
}