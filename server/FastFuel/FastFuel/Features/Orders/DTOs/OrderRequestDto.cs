using FastFuel.Features.OrderFoods.DTOs;
using FastFuel.Features.OrderMenus.DTOs;

namespace FastFuel.Features.Orders.DTOs;

public record OrderRequestDto
{
    /// <summary>
    ///     The restaurant identifier.
    /// </summary>
    public required uint RestaurantId { get; init; }

    /// <summary>
    ///     The list of menus.
    /// </summary>
    public required List<OrderMenuRequestDto> Menus { get; init; }

    /// <summary>
    ///     The list of foods.
    /// </summary>
    public required List<OrderFoodRequestDto> Foods { get; init; }
}