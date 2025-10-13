namespace FastFuel.Models;

public class MenuFood : BaseModel
{
    public uint MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;
    public uint FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;
    public uint Quantity { get; set; }
}