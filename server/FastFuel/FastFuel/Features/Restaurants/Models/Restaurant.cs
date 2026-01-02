using FastFuel.Features.Common;
using FastFuel.Features.OpeningHours.Models;

namespace FastFuel.Features.Restaurants.Models;

public class Restaurant : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public virtual List<OpeningHour> OpeningHours { get; init; } = [];
    public uint Id { get; init; }
}