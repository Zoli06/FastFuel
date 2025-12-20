using FastFuel.Features.Ingredients.Models;

namespace FastFuel.Features.Allergies.Models;

public class Allergy
{
    public uint Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string? Message { get; set; }
    public virtual List<Ingredient> Ingredients { get; init; } = [];
}