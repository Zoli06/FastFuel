using FastFuel.Features.Foods.Models;
using FastFuel.Features.Menus.Models;

namespace FastFuel.Features.MenuFoods.Models;

public class MenuFood
{
    public uint Id { get; set; }
    public uint MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;
    public uint FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;
    public uint Quantity { get; set; }
}