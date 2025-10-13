namespace FastFuel.Models;

public class FoodIngredient : BaseModel
{
    public uint FoodId { get; set; }
    public virtual Food Food { get; set; } = null!;
    public uint IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;
    public uint Quantity { get; set; }
}