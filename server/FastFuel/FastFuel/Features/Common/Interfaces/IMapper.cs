namespace FastFuel.Features.Common.Interfaces;

public interface IMapper<TModel, in TRequest, out TResponse>
{
    public TResponse ToDto(TModel model);
    public TModel ToModel(TRequest dto);
    public void UpdateModel(TRequest dto, ref TModel model);
}