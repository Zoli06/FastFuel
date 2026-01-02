using FastFuel.Features.Common;
using FastFuel.Features.Ingredients.Models;
using FastFuel.Features.Stations.Models;

namespace FastFuel.Features.StationCategories.Models;

public class StationCategory : IIdentifiable
{
    public uint Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public virtual List<Ingredient> Ingredients { get; init; } = [];
    public virtual List<Station> Stations { get; init; } = [];
}