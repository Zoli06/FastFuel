using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Orders.DTOs;

public record OrderResponseDto : IIdentifiable
{
    public required uint? CustomerId { get; set; }
    public required uint RestaurantId { get; init; }
    public required uint OrderNumber { get; init; }
    public required OrderStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? CompletedAt { get; init; }
    public required List<OrderMenuDto> Menus { get; init; }
    public required List<OrderFoodDto> Foods { get; init; }
    public required uint Price { get; init; }
    public required uint Id { get; init; }
}