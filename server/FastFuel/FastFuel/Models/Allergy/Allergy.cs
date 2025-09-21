namespace FastFuel.Models;

public class Allergy
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public string? Message { get; set; }
    public List<Ingredient> Ingredients { get; set; } = [];
}