using FastFuel.Features.Foods.Entities;
using FastFuel.Features.Ingredients.Entities;

namespace FastFuel.Features.FoodIngredients.Entities;

public class FoodIngredient
{
    public uint FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;
    public uint IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;
    public uint Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}