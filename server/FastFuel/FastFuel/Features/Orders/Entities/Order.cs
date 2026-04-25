using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.Users.Entities;

namespace FastFuel.Features.Orders.Entities;

public class Order : IIdentifiable
{
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; init; } = null!;
    public uint OrderNumber { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public uint UserId { get; set; }
    public virtual User User { get; init; } = null!;
    public virtual List<OrderFood> Foods { get; init; } = [];
    public virtual List<OrderMenu> Menus { get; init; } = [];
    public uint Id { get; init; }
}