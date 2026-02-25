using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class GetById<TEntity, TRequest, TResponse>(DbSet<TEntity> dbSet, IMapper<TEntity, TRequest, TResponse> mapper)
    where TEntity : class, IIdentifiable
{
    protected readonly DbSet<TEntity> DbSet = dbSet;
    protected readonly IMapper<TEntity, TRequest, TResponse> Mapper = mapper;

    protected virtual async Task<TEntity?> GetEntityAsync(uint id, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    protected virtual Task<TResponse> GetDtoAsync(uint id, TEntity entity, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Mapper.ToDto(entity));
    }

    public virtual async Task<TResponse?> ExecuteAsync(uint id, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(id, userId, cancellationToken);
        if (entity == null)
            return default;
        return await GetDtoAsync(id, entity, userId, cancellationToken);
    }
}