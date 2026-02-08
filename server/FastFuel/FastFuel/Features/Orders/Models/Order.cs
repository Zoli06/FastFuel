using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OrderFoods.Models;
using FastFuel.Features.OrderMenus.Models;
using FastFuel.Features.Restaurants.Models;
using FastFuel.Features.Users.Models;

namespace FastFuel.Features.Orders.Models;

public class Order : IIdentifiable
{
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; init; } = null!;
    public uint OrderNumber { get; set; }
    public OrderStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }

    public uint UserId { get; init; }
    public virtual User User { get; init; } = null!;
    public virtual List<OrderFood> Foods { get; init; } = [];
    public virtual List<OrderMenu> Menus { get; init; } = [];
    public uint Id { get; init; }
}