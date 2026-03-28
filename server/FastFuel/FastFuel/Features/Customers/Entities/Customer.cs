using FastFuel.Features.Orders.Entities;
using FastFuel.Features.Users.Entities;

namespace FastFuel.Features.Customers.Entities;

public class Customer : User
{
    public new string Email
    {
        get => base.Email ?? string.Empty;
        set => base.Email = value;
    }

    public virtual List<Order> Orders { get; init; } = [];
}