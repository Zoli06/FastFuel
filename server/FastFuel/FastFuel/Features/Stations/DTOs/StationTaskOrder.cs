using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Stations.DTOs;

public record StationTaskOrder
{
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public required uint Id { get; init; }
    /// <summary>
    /// The public order number displayed on the screens.
    /// </summary>
    public required uint OrderNumber { get; init; }
    /// <summary>
    /// The list of menus.
    /// </summary>
    public required List<StationTaskMenu> Menus { get; init; }
    /// <summary>
    /// The list of foods.
    /// </summary>
    public required List<StationTaskFood> Foods { get; init; }
    /// <summary>
    /// The date and time when the item was created.
    /// </summary>
    public required DateTime CreatedAt { get; init; }
    /// <summary>
    /// The status.
    /// </summary>
    public required OrderStatus Status { get; init; }
}