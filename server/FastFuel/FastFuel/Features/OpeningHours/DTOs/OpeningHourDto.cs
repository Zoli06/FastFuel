using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.OpeningHours.DTOs
{
    public class OpeningHourDto
    {
        public uint Id { get; set; }
        public uint RestaurantId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
    }
}
