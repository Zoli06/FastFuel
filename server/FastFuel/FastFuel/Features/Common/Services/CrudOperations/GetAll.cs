using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class GetAll<TEntity, TRequest, TResponse>(
    DbSet<TEntity> dbSet,
    IMapper<TEntity, TRequest, TResponse> mapper)
    where TEntity : class
{
    protected readonly DbSet<TEntity> DbSet = dbSet;
    protected readonly IMapper<TEntity, TRequest, TResponse> Mapper = mapper;

    protected virtual async Task<List<TEntity>> GetEntitiesAsync(uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    protected virtual Task<List<TResponse>> GetDtosAsync(List<TEntity> entities, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entities.ConvertAll(Mapper.ToDto));
    }

    public virtual async Task<List<TResponse>> ExecuteAsync(uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return await GetDtosAsync(await GetEntitiesAsync(userId, cancellationToken), userId, cancellationToken);
    }
}