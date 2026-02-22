using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class GetById<TModel, TRequest, TResponse>(DbSet<TModel> dbSet, IMapper<TModel, TRequest, TResponse> mapper)
    where TModel : class, IIdentifiable
{
    protected readonly DbSet<TModel> DbSet = dbSet;
    protected readonly IMapper<TModel, TRequest, TResponse> Mapper = mapper;

    protected virtual async Task<TModel?> GetModelAsync(uint id)
    {
        return await DbSet.FindAsync(id);
    }

    protected virtual Task<TResponse> GetDtoAsync(uint id, TModel model)
    {
        return Task.FromResult(Mapper.ToDto(model));
    }

    public virtual async Task<TResponse?> ExecuteAsync(uint id)
    {
        var model = await GetModelAsync(id);
        if (model == null)
            return default;
        return await GetDtoAsync(id, model);
    }
}