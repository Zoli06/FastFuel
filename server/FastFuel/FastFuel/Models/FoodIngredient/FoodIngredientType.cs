namespace FastFuel.Models;

public class FoodIngredientType : BaseType<FoodIngredient>
{
    protected override void Configure(IObjectTypeDescriptor<FoodIngredient> descriptor)
    {
        base.Configure(descriptor);
        descriptor.Field(fi => fi.FoodId).Ignore();
        descriptor.Field(fi => fi.IngredientId).Ignore();
    }
}