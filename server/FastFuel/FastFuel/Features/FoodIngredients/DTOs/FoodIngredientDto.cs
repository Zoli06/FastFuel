using FastFuel.Features.Foods.Models;
using FastFuel.Features.Ingredients.Models;

namespace FastFuel.Features.FoodIngredients.DTOs
{
    public class FoodIngredientDto
    {
        public uint FoodId { get; set; }
        public uint IngredientId { get; set; }
        public uint Quantity { get; set; }
    }
}
