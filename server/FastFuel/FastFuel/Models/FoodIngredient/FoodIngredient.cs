namespace FastFuel.Models;

public class FoodIngredient
{
    public uint FoodId { get; set; }
    public Food Food { get; set; } = null!;
    public uint IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public uint Quantity { get; set; }
}