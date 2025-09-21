namespace FastFuel.Models;

public class Ingredient
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint StationTypeId { get; set; }
    public StationType StationType { get; set; } = null!;
    public List<Food> Foods { get; set; } = [];
    public List<Allergy> Allergies { get; set; } = [];
}