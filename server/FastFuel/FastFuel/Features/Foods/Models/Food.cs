using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.MenuFoods.Models;

namespace FastFuel.Features.Foods.Models;

public class Food
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint Price { get; set; }
    public string? Description { get; set; }
    public Uri? ImageUrl { get; set; }
    public virtual List<MenuFood> MenuFoods { get; set; } = [];
    public virtual List<FoodIngredient> FoodIngredients { get; set; } = [];
}