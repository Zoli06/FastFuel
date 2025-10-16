namespace FastFuel.Models;

public class StationCategoryIngredient
{
    public uint StationCategoryId { get; set; }
    public virtual StationCategory StationCategory { get; set; } = null!;
    public uint IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;
}