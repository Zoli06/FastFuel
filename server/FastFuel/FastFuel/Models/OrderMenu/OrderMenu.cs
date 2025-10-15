namespace FastFuel.Models;

public class OrderMenu : BaseModel
{
    public uint OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    public uint MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;
    public uint Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}