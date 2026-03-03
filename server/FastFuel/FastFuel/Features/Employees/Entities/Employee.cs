using FastFuel.Features.Shifts.Entities;
using FastFuel.Features.StationCategories.Entities;
using FastFuel.Features.Users.Entities;

namespace FastFuel.Features.Employees.Entities;

public class Employee : User
{
    public virtual List<Shift> Shifts { get; set; } = [];
    public virtual List<StationCategory> StationCategories { get; set; } = [];
}