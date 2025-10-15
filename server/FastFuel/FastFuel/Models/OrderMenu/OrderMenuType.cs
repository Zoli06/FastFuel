namespace FastFuel.Models;

public class OrderMenuType : BaseType<OrderMenu>
{
    protected override void Configure(IObjectTypeDescriptor<OrderMenu> descriptor)
    {
        base.Configure(descriptor);
        descriptor.Field(om => om.OrderId).Ignore();
        descriptor.Field(om => om.MenuId).Ignore();
    }
}