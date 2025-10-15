namespace FastFuel.Models;

public class IngredientType : BaseType<Ingredient>
{
    protected override void Configure(IObjectTypeDescriptor<Ingredient> descriptor)
    {
        base.Configure(descriptor);
        descriptor.Field(i => i.StationTypeId).Ignore();
    }
}