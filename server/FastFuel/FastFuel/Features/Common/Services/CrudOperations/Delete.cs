using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class Delete<TEntity>(ApplicationDbContext dbContext, DbSet<TEntity> dbSet)
    where TEntity : class, IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly DbSet<TEntity> DbSet = dbSet;

    protected virtual async Task<TEntity?> GetEntityAsync(uint id)
    {
        return await DbSet.FindAsync(id);
    }

    protected virtual async Task DeleteEntityAsync(uint id, TEntity entity)
    {
        DbSet.Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task<bool> ExecuteAsync(uint id)
    {
        var entity = await GetEntityAsync(id);
        if (entity == null)
            return false;
        await DeleteEntityAsync(id, entity);
        return true;
    }
}