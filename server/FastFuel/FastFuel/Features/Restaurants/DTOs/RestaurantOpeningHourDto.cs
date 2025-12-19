namespace FastFuel.Features.Restaurants.DTOs;

public class RestaurantOpeningHourDto
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
}