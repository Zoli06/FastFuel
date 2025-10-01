namespace FastFuel.Models;

public class MenuFoodType : ObjectType<MenuFood>
{
    protected override void Configure(IObjectTypeDescriptor<MenuFood> descriptor)
    {
        descriptor.Field(mf => mf.MenuId).Ignore();
        descriptor.Field(mf => mf.FoodId).Ignore();
    }
}