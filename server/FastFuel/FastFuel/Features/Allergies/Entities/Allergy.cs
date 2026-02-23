using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Ingredients.Entities;

namespace FastFuel.Features.Allergies.Entities;

public class Allergy : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public string? Message { get; set; }
    public virtual List<Ingredient> Ingredients { get; init; } = [];
    public uint Id { get; init; }
}