namespace FastFuel.Features.Orders.DTOs;

public class OrderDto
{
    public uint Id { get; set; }
    public uint RestaurantId { get; set; }
    public uint OrderNumber { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<OrderMenuDto> Menus { get; set; } = [];
    public List<OrderFoodDto> Foods { get; set; } = [];
}