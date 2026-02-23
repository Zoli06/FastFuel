using FastFuel.Features.Shifts.Models;
using FastFuel.Features.StationCategories.Models;
using FastFuel.Features.Users.Models;

namespace FastFuel.Features.Employees.Models;

public class Employee : User
{
    public virtual List<Shift> Shifts { get; set; } = [];
    public virtual List<StationCategory> StationCategories { get; set; } = [];
}