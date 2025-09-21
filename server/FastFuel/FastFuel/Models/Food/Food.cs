namespace FastFuel.Models;

public class Food
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint Price { get; set; }
    public string? Description { get; set; }
    public List<Menu> Menus { get; set; } = [];
    public List<Ingredient> Ingredients { get; set; } = [];
}