using FastFuel.Features.Foods.Models;
using FastFuel.Features.Orders.Models;

namespace FastFuel.Features.OrderFoods.Models;

public class OrderFood
{
    public uint Id { get; set; }
    public uint OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;
    public uint FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;

    public uint Quantity { get; set; }
    public string? SpecialInstructions { get; set; }
}