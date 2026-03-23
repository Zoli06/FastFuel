using FastFuel.Features.Orders.Common;

namespace FastFuel.Features.Orders.Services.OrderFilter;

public class OrderFilterParams : IOrderFilterParams, IOrderFilterParamsFactory
{
    public OrderStatus? Status { get; set; }
    public uint? RestaurantId { get; set; }

    public bool TryParse(string? status, string? restaurantId, out IOrderFilterParams filterParams)
    {
        filterParams = new OrderFilterParams();

        if (!string.IsNullOrEmpty(status))
            if (Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                filterParams.Status = parsedStatus;
            else
                return false;

        if (!string.IsNullOrEmpty(restaurantId))
            if (uint.TryParse(restaurantId, out var parsedRestaurantId))
                filterParams.RestaurantId = parsedRestaurantId;
            else
                return false;

        return true;
    }
}