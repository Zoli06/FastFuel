using FastFuel.Features.Restaurants.Models;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.Stations.DTOs
{
    public class EditStationDto
    {
        public required string Name { get; set; }
        public bool InOperation { get; set; }
        public uint RestaurantId { get; set; }
        public uint StationCategoryId { get; set; }
    }
}
