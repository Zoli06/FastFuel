namespace FastFuel.Features.Common;

public abstract class Mapper<TModel, TRequest, TResponse>
{
    public abstract TResponse ToDto(TModel model);

    public abstract TModel ToModel(TRequest dto);

    public abstract void UpdateModel(TRequest dto, ref TModel model);
}