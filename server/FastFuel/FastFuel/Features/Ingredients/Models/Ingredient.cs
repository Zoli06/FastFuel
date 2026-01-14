using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common;
using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.Ingredients.Models;

public class Ingredient : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public Uri? ImageUrl { get; set; }
    public virtual List<FoodIngredient> FoodIngredients { get; init; } = [];
    public virtual List<Allergy> Allergies { get; init; } = [];
    public virtual List<StationCategory> StationCategories { get; init; } = [];
    public TimeSpan DefaultTimerValue { get; internal set; }
    public uint Id { get; init; }
}