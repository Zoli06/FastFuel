namespace FastFuel.Models;

public class OpeningHour
{
    public uint RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
}