using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class Delete<TEntity>(ApplicationDbContext dbContext, DbSet<TEntity> dbSet)
    where TEntity : class, IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly DbSet<TEntity> DbSet = dbSet;

    protected virtual async Task<TEntity?> GetEntityAsync(uint id, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    protected virtual async Task DeleteEntityAsync(uint id, TEntity entity, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<bool> ExecuteAsync(uint id, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        var entity = await GetEntityAsync(id, userId, cancellationToken);
        if (entity == null)
            return false;
        await DeleteEntityAsync(id, entity, userId, cancellationToken);
        return true;
    }
}