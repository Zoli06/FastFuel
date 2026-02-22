using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class Update<TModel, TRequest, TResponse>(
    ApplicationDbContext dbContext,
    DbSet<TModel> dbSet,
    IMapper<TModel, TRequest, TResponse> mapper)
    where TModel : class
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly DbSet<TModel> DbSet = dbSet;
    protected readonly IMapper<TModel, TRequest, TResponse> Mapper = mapper;

    protected virtual async Task<TModel?> GetModelAsync(uint id)
    {
        return await DbSet.FindAsync(id);
    }

    protected virtual Task UpdateModelAsync(uint id, TRequest requestDto, TModel model)
    {
        Mapper.UpdateModel(requestDto, model);
        return Task.CompletedTask;
    }

    protected virtual async Task SaveModelAsync(uint id, TRequest requestDto, TModel model)
    {
        await DbContext.SaveChangesAsync();
    }

    public virtual async Task<bool> ExecuteAsync(uint id, TRequest requestDto)
    {
        var model = await GetModelAsync(id);
        if (model == null)
            return false;
        await UpdateModelAsync(id, requestDto, model);
        await SaveModelAsync(id, requestDto, model);
        return true;
    }
}