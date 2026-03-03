using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.MenuFoods.Entities;

namespace FastFuel.Features.Menus.Entities;

public class Menu : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public virtual List<MenuFood> MenuFoods { get; init; } = [];
    public uint Id { get; init; }
}