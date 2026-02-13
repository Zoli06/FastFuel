using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Orders.Models;
using FastFuel.Features.Themes.Models;

namespace FastFuel.Features.Employees.Models;

public class Employee : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public virtual List<Order> Orders { get; init; } = [];
    public uint ThemeId { get; set; }
    public virtual Theme Theme { get; set; } = null!;
    public string PasswordHash { get; set; } = string.Empty;
    public uint Id { get; init; }
}