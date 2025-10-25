using FastFuel.Features.Ingredients.Models;
using FastFuel.Features.Stations.Models;

namespace FastFuel.Features.StationCategories.Models;

public class StationCategory
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public virtual List<Ingredient> Ingredients { get; set; } = [];
    public virtual List<Station> Stations { get; set; } = [];
}