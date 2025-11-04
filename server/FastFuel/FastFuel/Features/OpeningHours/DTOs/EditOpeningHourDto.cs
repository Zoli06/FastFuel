using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.OpeningHours.DTOs
{
    public class EditOpeningHourDto
    {
        public uint RestaurantId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
    }
}
