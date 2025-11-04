using FastFuel.Features.Allergies.Models;
using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.Ingredients.Models;

public class Ingredient
{
    public uint Id { get; set; }
    public required string Name { get; set; }
    public uint StationTypeId { get; set; }
    public Uri? ImageUrl { get; set; }
    public virtual List<FoodIngredient> FoodIngredients { get; set; } = [];
    public virtual List<Allergy> Allergies { get; set; } = [];
    public virtual List<StationCategory> StationCategories { get; set; } = [];
}