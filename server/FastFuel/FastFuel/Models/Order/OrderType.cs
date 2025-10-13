namespace FastFuel.Models;

public class OrderType : BaseType<Order>
{
    protected override void Configure(IObjectTypeDescriptor<Order> descriptor)
    {
        base.Configure(descriptor);
        descriptor.Field(o => o.RestaurantId).Ignore();
    }
}