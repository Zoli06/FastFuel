namespace FastFuel.Models;

public enum OrderStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public class Order
{
    public uint Id { get; set; }
    public uint RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;
    public uint OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}