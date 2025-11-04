using FastFuel.Features.Orders.Models;
using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.Orders.DTOs
{
    public class OrderDto
    {
        public uint Id { get; set; }
        public uint RestaurantId { get; set; }
        public uint OrderNumber { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
