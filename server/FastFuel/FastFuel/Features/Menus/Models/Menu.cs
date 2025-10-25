using FastFuel.Features.MenuFoods.Models;

namespace FastFuel.Features.Menus.Models;

public class Menu
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public virtual List<MenuFood> MenuFoods { get; set; } = [];
}