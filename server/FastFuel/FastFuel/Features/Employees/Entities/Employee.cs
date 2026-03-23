using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.Shifts.Entities;
using FastFuel.Features.StationCategories.Entities;
using FastFuel.Features.Users.Entities;

namespace FastFuel.Features.Employees.Entities;

public class Employee : User
{
    public new string Email
    {
        get => base.Email ?? string.Empty;
        set => base.Email = value;
    }

    public virtual List<Shift> Shifts { get; set; } = [];
    public virtual List<StationCategory> StationCategories { get; set; } = [];
    public uint WorksAtRestaurantId { get; set; }
    public virtual Restaurant WorksAtRestaurant { get; set; } = null!;
}