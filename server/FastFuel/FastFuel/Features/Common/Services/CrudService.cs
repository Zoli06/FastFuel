using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services.CrudOperations;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services;

public abstract class CrudService<TEntity, TRequest, TResponse>(
    ApplicationDbContext dbContext,
    IMapper<TEntity, TRequest, TResponse> mapper)
    : ICrudService<TRequest, TResponse>
    where TEntity : class, IIdentifiable
    where TRequest : class
    where TResponse : class, IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly IMapper<TEntity, TRequest, TResponse> Mapper = mapper;
    protected abstract DbSet<TEntity> DbSet { get; }

    protected virtual GetAll<TEntity, TRequest, TResponse> GetAllOperation => new(DbSet, Mapper);
    protected virtual GetById<TEntity, TRequest, TResponse> GetByIdOperation => new(DbSet, Mapper);
    protected virtual Create<TEntity, TRequest, TResponse> CreateOperation => new(DbContext, DbSet, Mapper);
    protected virtual Update<TEntity, TRequest, TResponse> UpdateOperation => new(DbContext, DbSet, Mapper);
    protected virtual Delete<TEntity> DeleteOperation => new(DbContext, DbSet);

    public Task<List<TResponse>> GetAllAsync()
    {
        return GetAllOperation.ExecuteAsync();
    }

    public Task<TResponse?> GetByIdAsync(uint id)
    {
        return GetByIdOperation.ExecuteAsync(id);
    }

    public Task<TResponse> CreateAsync(TRequest requestDto)
    {
        return CreateOperation.ExecuteAsync(requestDto);
    }

    public Task<bool> UpdateAsync(uint id, TRequest requestDto)
    {
        return UpdateOperation.ExecuteAsync(id, requestDto);
    }

    public Task<bool> DeleteAsync(uint id)
    {
        return DeleteOperation.ExecuteAsync(id);
    }
}