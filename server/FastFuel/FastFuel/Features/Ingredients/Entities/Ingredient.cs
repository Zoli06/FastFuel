using FastFuel.Features.Allergies.Entities;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.FoodIngredients.Entities;
using FastFuel.Features.StationCategories.Entities;

namespace FastFuel.Features.Ingredients.Entities;

public class Ingredient : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public virtual List<FoodIngredient> FoodIngredients { get; init; } = [];
    public virtual List<Allergy> Allergies { get; init; } = [];
    public virtual List<StationCategory> StationCategories { get; init; } = [];
    public uint Id { get; init; }
}