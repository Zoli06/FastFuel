namespace FastFuel.Models;

public class OpeningHourType : BaseType<OpeningHour>
{
    protected override void Configure(IObjectTypeDescriptor<OpeningHour> descriptor)
    {
        base.Configure(descriptor);
        descriptor.Field(oh => oh.RestaurantId).Ignore();
    }
}