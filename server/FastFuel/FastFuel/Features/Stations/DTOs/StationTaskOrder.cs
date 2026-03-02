using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Stations.DTOs;

public record StationTaskOrder
{
    public required uint Id { get; init; }
    public required uint OrderNumber { get; init; }
    public required List<StationTaskMenu> Menus { get; init; }
    public required List<StationTaskFood> Foods { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required OrderStatus Status { get; init; }
}