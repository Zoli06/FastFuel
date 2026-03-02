using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Orders.Services.OrderFilter;

public class OrderFilterParams : IOrderFilterParams, IOrderFilterParamsFactory
{
    public OrderStatus? Status { get; set; }

    public bool TryParse(string? status, out IOrderFilterParams filterParams)
    {
        filterParams = new OrderFilterParams();

        if (!string.IsNullOrEmpty(status))
            if (Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                filterParams.Status = parsedStatus;
            else
                return false;

        return true;
    }
}