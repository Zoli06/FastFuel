using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Restaurants.Entities;

namespace FastFuel.Features.Orders.Entities;

public class Order : IIdentifiable
{
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; init; } = null!;
    public uint OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public uint? CustomerId { get; set; }
    public virtual Customer? Customer { get; init; }
    public virtual List<OrderFood> Foods { get; init; } = [];
    public virtual List<OrderMenu> Menus { get; init; } = [];
    public uint Price { get; set; }
    public uint Id { get; init; }
}