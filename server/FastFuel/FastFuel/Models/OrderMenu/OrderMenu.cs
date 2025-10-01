namespace FastFuel.Models;

public class OrderMenu
{
    public uint Id { get; set; }
    public uint OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    public uint MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;
    public uint Quantity { get; set; }
    public virtual List<OrderFood> OrderFoods { get; set; } = [];
}