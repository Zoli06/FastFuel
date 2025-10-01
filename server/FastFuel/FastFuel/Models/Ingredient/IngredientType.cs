namespace FastFuel.Models;

public class IngredientType : ObjectType<Ingredient>
{
    protected override void Configure(IObjectTypeDescriptor<Ingredient> descriptor)
    {
        descriptor.Field(i => i.StationTypeId).Ignore();
    }
}