namespace FastFuel.Features.Common.Interfaces;

public interface IMapper<TModel, in TRequest, out TResponse> where TModel : class
{
    TResponse ToDto(TModel model);
    TModel ToModel(TRequest dto);
    void UpdateModel(TRequest dto, TModel model);
}