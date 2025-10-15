namespace FastFuel.Models;

public abstract class BaseType<T> : ObjectType<T> where T : BaseModel
{
    protected override void Configure(IObjectTypeDescriptor<T> descriptor)
    {
        descriptor.Field(b => b.Id).ID();
    }
}