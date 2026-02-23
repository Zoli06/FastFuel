using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Restaurants.Entities;

namespace FastFuel.Features.Orders.Entities;

public class Order : IIdentifiable
{
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; init; } = null!;
    public uint OrderNumber { get; set; }
    public OrderStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }

    public uint? CustomerId { get; init; }
    public virtual Customer? Customer { get; init; }
    public virtual List<OrderFood> Foods { get; init; } = [];
    public virtual List<OrderMenu> Menus { get; init; } = [];
    public uint Id { get; init; }
}