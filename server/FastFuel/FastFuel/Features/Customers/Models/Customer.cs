using FastFuel.Features.Orders.Models;
using FastFuel.Features.Users.Models;

namespace FastFuel.Features.Customers.Models;

public class Customer : User
{
    public virtual List<Order> Orders { get; init; } = [];
}