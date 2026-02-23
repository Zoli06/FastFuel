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

    protected virtual async Task<TEntity?> GetEntityAsync(uint id)
    {
        return await DbSet.FindAsync(id);
    }

    protected virtual Task UpdateEntityAsync(uint id, TRequest requestDto, TEntity entity)
    {
        Mapper.UpdateEntity(requestDto, entity);
        return Task.CompletedTask;
    }

    protected virtual async Task SaveEntityAsync(uint id, TRequest requestDto, TEntity entity)
    {
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task<bool> ExecuteAsync(uint id, TRequest requestDto)
    {
        var entity = await GetEntityAsync(id);
        if (entity == null)
            return false;
        await UpdateEntityAsync(id, requestDto, entity);
        await SaveEntityAsync(id, requestDto, entity);
        return true;
    }
}