using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Orders.DTOs;

public record OrderResponseDto : IIdentifiable
{
    /// <summary>
    /// The user identifier.
    /// </summary>
    public required uint UserId { get; set; }
    /// <summary>
    /// The restaurant identifier.
    /// </summary>
    public required uint RestaurantId { get; init; }
    /// <summary>
    /// The public order number.
    /// </summary>
    public required uint OrderNumber { get; init; }
    /// <summary>
    /// The status of the order.
    /// </summary>
    public required OrderStatus Status { get; init; }
    /// <summary>
    /// The date and time when the item was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
    /// <summary>
    /// When the order is completed at.
    /// </summary>
    public required DateTime? CompletedAt { get; init; }
    /// <summary>
    /// The list of menus.
    /// </summary>
    public required List<OrderMenuDto> Menus { get; init; }
    /// <summary>
    /// The list of foods.
    /// </summary>
    public required List<OrderFoodDto> Foods { get; init; }
    /// <summary>
    /// The final price.
    /// </summary>
    public required uint Price { get; init; }
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
}