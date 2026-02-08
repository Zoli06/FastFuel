namespace FastFuel.Features.Restaurants.DTOs;

public class RestaurantOpeningHourDto
{
    public DayOfWeek DayOfWeek { get; init; }
    public TimeOnly OpenTime { get; init; }
    public TimeOnly CloseTime { get; init; }
}