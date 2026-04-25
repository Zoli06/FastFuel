using FastFuel.Features.Menus.Entities;
using FastFuel.Features.Orders.Entities;

namespace FastFuel.Features.OrderMenus.Entities;

public class OrderMenu
{
    public uint Id { get; set; }
    public uint OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    public string OriginalMenuName { get; set; } = string.Empty;
    public double OriginalMenuPrice { get; set; }
    public uint MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;
    public uint Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}