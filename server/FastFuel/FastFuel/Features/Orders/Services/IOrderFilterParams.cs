using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Orders.Services;

public interface IOrderFilterParams
{
    OrderStatus? Status { get; set; }
}