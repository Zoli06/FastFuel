namespace FastFuel.Models;

public class OrderMenuType : ObjectType<OrderMenu>
{
    protected override void Configure(IObjectTypeDescriptor<OrderMenu> descriptor)
    {
        descriptor.Field(om => om.OrderId).Ignore();
        descriptor.Field(om => om.MenuId).Ignore();
    }
}