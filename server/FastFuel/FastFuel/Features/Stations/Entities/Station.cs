using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.StationCategories.Entities;

namespace FastFuel.Features.Stations.Entities;

public class Station : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public bool InOperation { get; set; }
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    public uint StationCategoryId { get; set; }
    public virtual StationCategory StationCategory { get; set; } = null!;
    public uint Id { get; init; }
}