using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Orders.Services.OrderFilter;

public interface IOrderFilterParams
{
    OrderStatus? Status { get; set; }
}