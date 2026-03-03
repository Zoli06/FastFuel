namespace FastFuel.Features.Common.Interfaces;

public interface IMapper<TEntity, in TRequest, out TResponse> where TEntity : class
{
    TResponse ToDto(TEntity entity);
    TEntity ToEntity(TRequest dto);
    void UpdateEntity(TRequest dto, TEntity entity);
}