using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Orders.DTOs;

public class OrderResponseDto : IIdentifiable
{
    public uint UserId { get; set; }
    public uint RestaurantId { get; init; }
    public uint OrderNumber { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public List<OrderMenuDto> Menus { get; init; } = [];
    public List<OrderFoodDto> Foods { get; init; } = [];
    public uint Id { get; init; }
}