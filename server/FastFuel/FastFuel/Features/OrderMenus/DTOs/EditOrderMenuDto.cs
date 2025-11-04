using FastFuel.Features.Menus.Models;
using FastFuel.Features.Orders.Models;

namespace FastFuel.Features.OrderMenus.DTOs
{
    public class EditOrderMenuDto
    {
        public uint Id { get; set; }
        public uint OrderId { get; set; }
        public uint MenuId { get; set; }
        public uint Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}
