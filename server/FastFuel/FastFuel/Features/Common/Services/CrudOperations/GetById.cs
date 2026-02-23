using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class GetById<TEntity, TRequest, TResponse>(DbSet<TEntity> dbSet, IMapper<TEntity, TRequest, TResponse> mapper)
    where TEntity : class, IIdentifiable
{
    protected readonly DbSet<TEntity> DbSet = dbSet;
    protected readonly IMapper<TEntity, TRequest, TResponse> Mapper = mapper;

    protected virtual async Task<TEntity?> GetEntityAsync(uint id)
    {
        return await DbSet.FindAsync(id);
    }

    protected virtual Task<TResponse> GetDtoAsync(uint id, TEntity entity)
    {
        return Task.FromResult(Mapper.ToDto(entity));
    }

    public virtual async Task<TResponse?> ExecuteAsync(uint id)
    {
        var entity = await GetEntityAsync(id);
        if (entity == null)
            return default;
        return await GetDtoAsync(id, entity);
    }
}