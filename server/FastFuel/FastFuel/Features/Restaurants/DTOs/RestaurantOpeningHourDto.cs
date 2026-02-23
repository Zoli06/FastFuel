namespace FastFuel.Features.Restaurants.DTOs;

public record RestaurantOpeningHourDto
{
    public required DayOfWeek DayOfWeek { get; init; }
    public required TimeOnly OpenTime { get; init; }
    public required TimeOnly CloseTime { get; init; }
}