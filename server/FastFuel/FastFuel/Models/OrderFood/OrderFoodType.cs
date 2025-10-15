namespace FastFuel.Models;

public class OrderFoodType : BaseType<OrderFood>
{
    protected override void Configure(IObjectTypeDescriptor<OrderFood> descriptor)
    {
        base.Configure(descriptor);
        descriptor.Field(of => of.OrderId).Ignore();
        descriptor.Field(of => of.FoodId).Ignore();
    }
}