namespace FastFuel.Features.Orders.DTOs;

public class OrderCreateAtWorkPlaceRequestDto
{
    /// <summary>
    /// The list of menus in the order.
    /// </summary>
    public required List<OrderMenuDto> Menus { get; init; }
    /// <summary>
    /// The list of foods in the order.
    /// </summary>
    public required List<OrderFoodDto> Foods { get; init; }
}