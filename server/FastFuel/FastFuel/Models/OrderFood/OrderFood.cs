namespace FastFuel.Models;

public class OrderFood
{
    public uint Id { get; set; }
    public uint OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    public uint FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;

    public uint Quantity { get; set; }
    // Maybe don't do this, don't complicate things
    // public uint? OrderMenuId { get; set; }
    // public OrderMenu? OrderMenu { get; set; }
}