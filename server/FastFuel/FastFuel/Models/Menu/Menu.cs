namespace FastFuel.Models;

public class Menu
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint Price { get; set; }
    public bool IsSpecialDeal { get; set; }
    public string? Description { get; set; }
    public List<Food> Foods { get; set; } = [];
}