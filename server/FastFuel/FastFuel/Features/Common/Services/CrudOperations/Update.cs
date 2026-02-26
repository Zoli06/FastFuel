using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class Update<TEntity, TRequest, TResponse>(
    ApplicationDbContext dbContext,
    DbSet<TEntity> dbSet,
    IMapper<TEntity, TRequest, TResponse> mapper)
    where TEntity : class
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly DbSet<TEntity> DbSet = dbSet;
    protected readonly IMapper<TEntity, TRequest, TResponse> Mapper = mapper;

    protected virtual async Task<TEntity?> GetEntityAsync(uint id, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    protected virtual Task UpdateEntityAsync(uint id, TRequest requestDto, TEntity entity, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        Mapper.UpdateEntity(requestDto, entity);
        return Task.CompletedTask;
    }

    protected virtual async Task SaveEntityAsync(uint id, TRequest requestDto, TEntity entity, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> ExecuteAsync(uint id, TRequest requestDto, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(id, userId, cancellationToken);
        if (entity == null)
            return false;
        await UpdateEntityAsync(id, requestDto, entity, userId, cancellationToken);
        await SaveEntityAsync(id, requestDto, entity, userId, cancellationToken);
        return true;
    }
}