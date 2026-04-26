using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Orders.Services;

public class OrderFilterParams
{
    public OrderStatus? Status { get; set; }
    public uint? RestaurantId { get; set; }
}