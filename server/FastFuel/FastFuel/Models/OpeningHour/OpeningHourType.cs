namespace FastFuel.Models;

public class OpeningHourType : ObjectType<OpeningHour>
{
    protected override void Configure(IObjectTypeDescriptor<OpeningHour> descriptor)
    {
        descriptor.Field(oh => oh.RestaurantId).Ignore();
    }
}