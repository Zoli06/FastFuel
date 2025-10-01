namespace FastFuel.Models;

public class Ingredient
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint StationTypeId { get; set; }
    public virtual StationCategory StationCategory { get; set; } = null!;
    public virtual List<Food> Foods { get; set; } = [];
    public virtual List<Allergy> Allergies { get; set; } = [];
}