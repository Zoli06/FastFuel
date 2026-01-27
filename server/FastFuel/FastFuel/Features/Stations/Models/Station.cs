using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.Models;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.Stations.Models;

public class Station : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public bool InOperation { get; set; }
    public uint RestaurantId { get; set; }
    public virtual Restaurant Restaurant { get; set; } = null!;
    public uint StationCategoryId { get; set; }
    public virtual StationCategory StationCategory { get; set; } = null!;//delete this comment
    public uint Id { get; init; }
}