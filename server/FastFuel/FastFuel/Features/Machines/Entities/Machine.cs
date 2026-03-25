using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.Users.Entities;

namespace FastFuel.Features.Machines.Entities;

public class Machine : User
{
    public uint LocatedAtRestaurantId { get; set; }
    public virtual Restaurant LocatedAtRestaurant { get; set; } = null!;
}