namespace FastFuel.Features.Common.Interfaces;

public interface IMapper<TModel, in TRequest, out TResponse>
{
    TResponse ToDto(TModel model);
    TModel ToModel(TRequest dto);
    void UpdateModel(TRequest dto, ref TModel model);
}