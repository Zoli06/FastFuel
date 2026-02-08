using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OpeningHours.Models;

namespace FastFuel.Features.Restaurants.Models
{
    public class Restaurant : IIdentifiable
    {
        public uint Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Phone { get; set; }
        public virtual List<OpeningHour> OpeningHours { get; init; } = [];
        public string PasswordHash { get; set; } = string.Empty;
    }
}