namespace FastFuel.Models;

public class SpecialRequestType : ObjectType<SpecialRequest>
{
    protected override void Configure(IObjectTypeDescriptor<SpecialRequest> descriptor)
    {
        descriptor.Field(sr => sr.FoodId).Ignore();
    }
}