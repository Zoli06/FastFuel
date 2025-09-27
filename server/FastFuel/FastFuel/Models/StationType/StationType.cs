namespace FastFuel.Models;

public class StationType
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public List<Ingredient> Ingredients { get; set; } = [];
    public List<Station> Stations { get; set; } = [];
}