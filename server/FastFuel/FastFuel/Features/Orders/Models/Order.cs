using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.Orders.Models;

public class Order
{
    public uint Id { get; set; }
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    public uint OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}