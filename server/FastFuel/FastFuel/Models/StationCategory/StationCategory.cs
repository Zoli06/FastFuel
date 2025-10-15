namespace FastFuel.Models;

public class StationCategory : BaseModel
{
    public required string Name { get; set; }
    public virtual List<Ingredient> Ingredients { get; set; } = [];
    public virtual List<Station> Stations { get; set; } = [];
}