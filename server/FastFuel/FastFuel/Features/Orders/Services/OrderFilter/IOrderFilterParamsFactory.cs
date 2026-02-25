namespace FastFuel.Features.Orders.Services.OrderFilter;

public interface IOrderFilterParamsFactory
{
    bool TryParse(string? status, out IOrderFilterParams filterParams);
}