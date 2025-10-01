namespace FastFuel.Models;

public class StationType : ObjectType<Station>
{
    protected override void Configure(IObjectTypeDescriptor<Station> descriptor)
    {
        descriptor.Field(s => s.RestaurantId).Ignore();
        descriptor.Field(s => s.StationCategoryId).Ignore();
    }
}