using FastFuel.Features.Foods.Models;
using FastFuel.Features.Orders.Models;

namespace FastFuel.Features.OrderFoods.DTOs
{
    public class EditOrderFoodDto
    {
        public uint OrderId { get; set; }
        public uint FoodId { get; set; }
        public uint Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}
