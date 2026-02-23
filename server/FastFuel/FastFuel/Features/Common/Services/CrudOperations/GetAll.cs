using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class GetAll<TModel, TRequest, TResponse>(
    DbSet<TModel> dbSet,
    IMapper<TModel, TRequest, TResponse> mapper)
    where TModel : class
{
    protected readonly DbSet<TModel> DbSet = dbSet;
    protected readonly IMapper<TModel, TRequest, TResponse> Mapper = mapper;

    protected virtual async Task<List<TModel>> GetModelsAsync()
    {
        return await DbSet.ToListAsync();
    }

    protected virtual Task<List<TResponse>> GetDtosAsync(List<TModel> models)
    {
        return Task.FromResult(models.ConvertAll(Mapper.ToDto));
    }

    public virtual Task<List<TResponse>> ExecuteAsync()
    {
        return GetDtosAsync(GetModelsAsync().Result);
    }
}