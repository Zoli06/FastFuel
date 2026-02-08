using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.MenuFoods.Models;

namespace FastFuel.Features.Foods.Models;

public class Food : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public virtual List<MenuFood> MenuFoods { get; init; } = [];
    public virtual List<FoodIngredient> FoodIngredients { get; init; } = [];
    public uint Id { get; init; }
}