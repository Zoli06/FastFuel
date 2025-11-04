using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.OpeningHours.DTOs
{
    public class EditOpeningHourDto
    {
        public uint RestaurantId { get; set; }
        public virtual Restaurant Restaurant { get; set; } = null!;
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
    }
}
