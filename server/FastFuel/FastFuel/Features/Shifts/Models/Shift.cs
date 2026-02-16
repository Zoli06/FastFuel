using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Employees.Models;

namespace FastFuel.Features.Shifts.Models;

public class Shift : IIdentifiable
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public uint EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;
    public uint Id { get; init; }
}