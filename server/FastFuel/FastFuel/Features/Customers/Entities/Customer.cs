using FastFuel.Features.Orders.Entities;
using FastFuel.Features.Users.Entities;

namespace FastFuel.Features.Customers.Entities;

public class Customer : User
{
    public virtual List<Order> Orders { get; init; } = [];
}