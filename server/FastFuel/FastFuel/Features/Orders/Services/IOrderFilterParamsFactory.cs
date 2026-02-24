namespace FastFuel.Features.Orders.Services;

public interface IOrderFilterParamsFactory
{
    bool TryParse(string? status, out IOrderFilterParams filterParams);
}