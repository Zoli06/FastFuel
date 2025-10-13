namespace FastFuel.Models;

public class StationType : BaseType<Station>
{
    protected override void Configure(IObjectTypeDescriptor<Station> descriptor)
    {
        base.Configure(descriptor);
        descriptor.Field(s => s.RestaurantId).Ignore();
        descriptor.Field(s => s.StationCategoryId).Ignore();
    }
}