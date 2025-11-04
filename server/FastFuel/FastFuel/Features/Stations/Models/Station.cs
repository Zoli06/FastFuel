using FastFuel.Features.Restaurants.Models;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.Stations.Models;

public class Station
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public bool InOperation { get; set; }
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    public uint StationCategoryId { get; set; }
    public virtual StationCategory StationCategory { get; set; } = null!;
}