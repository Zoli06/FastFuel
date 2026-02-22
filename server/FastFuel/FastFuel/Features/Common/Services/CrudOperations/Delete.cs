using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class Delete<TModel>(ApplicationDbContext dbContext, DbSet<TModel> dbSet)
    where TModel : class, IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly DbSet<TModel> DbSet = dbSet;

    protected virtual async Task<TModel?> GetModelAsync(uint id)
    {
        return await DbSet.FindAsync(id);
    }

    protected virtual async Task DeleteModelAsync(uint id, TModel model)
    {
        DbSet.Remove(model);
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task<bool> ExecuteAsync(uint id)
    {
        var model = await GetModelAsync(id);
        if (model == null)
            return false;
        await DeleteModelAsync(id, model);
        return true;
    }
}