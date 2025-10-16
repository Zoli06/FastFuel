namespace FastFuel.Models;

public class IngredientAllergy : BaseModel
{
    public uint IngredientId { get; set; }
    public virtual Ingredient Ingredient { get; set; } = null!;
    public uint AllergyId { get; set; }
    public virtual Allergy Allergy { get; set; } = null!;
}