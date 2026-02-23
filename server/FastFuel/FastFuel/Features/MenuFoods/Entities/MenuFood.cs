using FastFuel.Features.Foods.Entities;
using FastFuel.Features.Menus.Entities;

namespace FastFuel.Features.MenuFoods.Entities;

public class MenuFood
{
    public uint MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;
    public uint FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;
    public uint Quantity { get; set; }
}