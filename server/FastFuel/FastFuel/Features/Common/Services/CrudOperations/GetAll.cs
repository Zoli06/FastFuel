using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class GetAll<TEntity, TRequest, TResponse>(
    DbSet<TEntity> dbSet,
    IMapper<TEntity, TRequest, TResponse> mapper)
    where TEntity : class
{
    protected readonly DbSet<TEntity> DbSet = dbSet;
    protected readonly IMapper<TEntity, TRequest, TResponse> Mapper = mapper;

    protected virtual async Task<List<TEntity>> GetEntitesAsync()
    {
        return await DbSet.ToListAsync();
    }

    protected virtual Task<List<TResponse>> GetDtosAsync(List<TEntity> entites)
    {
        return Task.FromResult(entites.ConvertAll(Mapper.ToDto));
    }

    public virtual Task<List<TResponse>> ExecuteAsync()
    {
        return GetDtosAsync(GetEntitesAsync().Result);
    }
}