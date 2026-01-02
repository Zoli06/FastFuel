using FastFuel.Features.Common;
using FastFuel.Features.MenuFoods.Models;

namespace FastFuel.Features.Menus.Models;

public class Menu : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public virtual List<MenuFood> MenuFoods { get; init; } = [];
    public uint Id { get; init; }
}