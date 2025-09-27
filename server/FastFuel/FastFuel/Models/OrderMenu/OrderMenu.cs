namespace FastFuel.Models;

public class OrderMenu
{
    public uint Id { get; set; }
    public uint OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public uint MenuId { get; set; }
    public Menu Menu { get; set; } = null!;
    public uint Quantity { get; set; }
    public List<OrderFood> OrderFoods { get; set; } = [];
}