namespace FastFuel.Models;

public class Menu : BaseModel
{
    public required string Name { get; set; }
    public uint Price { get; set; }
    public bool IsSpecialDeal { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public virtual List<Food> Foods { get; set; } = [];
}