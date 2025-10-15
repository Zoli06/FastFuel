namespace FastFuel.Models;

public class Allergy : BaseModel
{
    public required string Name { get; set; }
    public string? Message { get; set; }
    public virtual List<Ingredient> Ingredients { get; set; } = [];
}