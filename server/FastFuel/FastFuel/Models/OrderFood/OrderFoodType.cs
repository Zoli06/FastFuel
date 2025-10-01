namespace FastFuel.Models;

public class OrderFoodType : ObjectType<OrderFood>
{
    protected override void Configure(IObjectTypeDescriptor<OrderFood> descriptor)
    {
        descriptor.Field(of => of.OrderId).Ignore();
        descriptor.Field(of => of.FoodId).Ignore();
        // descriptor.Field(of => of.OrderMenuId).Ignore();
    }
}