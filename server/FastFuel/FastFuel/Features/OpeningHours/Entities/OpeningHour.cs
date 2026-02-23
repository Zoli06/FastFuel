using FastFuel.Features.Restaurants.Entities;

namespace FastFuel.Features.OpeningHours.Entities;

public class OpeningHour
{
    public uint Id { get; set; }
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
}