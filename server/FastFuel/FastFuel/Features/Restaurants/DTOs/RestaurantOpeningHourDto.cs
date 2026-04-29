namespace FastFuel.Features.Restaurants.DTOs;

public record RestaurantOpeningHourDto
{
    /// <summary>
    /// The day of week.
    /// </summary>
    public required DayOfWeek DayOfWeek { get; init; }
    /// <summary>
    /// The open time.
    /// </summary>
    public required TimeOnly OpenTime { get; init; }
    /// <summary>
    /// The close time.
    /// </summary>
    public required TimeOnly CloseTime { get; init; }
}