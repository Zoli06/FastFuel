namespace FastFuel.Models;

public class MenuFood
{
    public uint MenuId { get; set; }
    public Menu Menu { get; set; } = null!;
    public uint FoodId { get; set; }
    public Food Food { get; set; } = null!;
    public uint Quantity { get; set; }
}