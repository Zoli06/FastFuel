namespace FastFuel.Features.Orders.Services.OrderFilter;

public interface IOrderFilterParamsFactory
{
    bool TryParse(string? status, string? restaurantId, out IOrderFilterParams filterParams);
}