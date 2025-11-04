using FastFuel.Features.Ingredients.Models;

namespace FastFuel.Features.Allergies.Models;

public class Allergy
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public string? Message { get; set; }
    public virtual List<Ingredient> Ingredients { get; set; } = [];
}