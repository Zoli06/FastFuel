namespace FastFuel.Models;

public class FoodIngredientType : ObjectType<FoodIngredient>
{
    protected override void Configure(IObjectTypeDescriptor<FoodIngredient> descriptor)
    {
        descriptor.Field(fi => fi.FoodId).Ignore();
        descriptor.Field(fi => fi.IngredientId).Ignore();
    }
}