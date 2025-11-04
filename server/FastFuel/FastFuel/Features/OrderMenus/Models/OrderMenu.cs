using FastFuel.Features.Menus.Models;
using FastFuel.Features.Orders.Models;

namespace FastFuel.Features.OrderMenus.Models;

public class OrderMenu
{
    public uint Id { get; set; }
    public uint OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    public uint MenuId { get; set; }
    public virtual Menu Menu { get; set; } = null!;
    public uint Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}