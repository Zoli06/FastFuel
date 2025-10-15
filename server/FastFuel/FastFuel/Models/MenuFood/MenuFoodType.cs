namespace FastFuel.Models;

public class MenuFoodType : BaseType<MenuFood>
{
    protected override void Configure(IObjectTypeDescriptor<MenuFood> descriptor)
    {
        base.Configure(descriptor);
        descriptor.Field(mf => mf.MenuId).Ignore();
        descriptor.Field(mf => mf.FoodId).Ignore();
    }
}