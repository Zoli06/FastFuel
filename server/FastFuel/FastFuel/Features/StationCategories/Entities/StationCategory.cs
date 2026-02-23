using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Ingredients.Entities;
using FastFuel.Features.Stations.Entities;

namespace FastFuel.Features.StationCategories.Entities;

public class StationCategory : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public virtual List<Ingredient> Ingredients { get; init; } = [];
    public virtual List<Station> Stations { get; init; } = [];
    public virtual List<Employee> Employees { get; init; } = [];
    public uint Id { get; init; }
}