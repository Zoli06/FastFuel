namespace FastFuel.Models;

public class OrderFood
{
    public uint Id { get; set; }
    public uint OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public uint FoodId { get; set; }
    public Food Food { get; set; } = null!;
    public uint Quantity { get; set; }
    public uint? OrderMenuId { get; set; }
    public OrderMenu? OrderMenu { get; set; }
}