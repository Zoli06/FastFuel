using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services.CrudOperations;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services;

public abstract class CrudService<TModel, TRequest, TResponse>(
    ApplicationDbContext dbContext,
    IMapper<TModel, TRequest, TResponse> mapper)
    : ICrudService<TRequest, TResponse>
    where TModel : class, IIdentifiable
    where TRequest : class
    where TResponse : class, IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly IMapper<TModel, TRequest, TResponse> Mapper = mapper;
    protected abstract DbSet<TModel> DbSet { get; }

    protected virtual GetAll<TModel, TRequest, TResponse> GetAllOperation => new(DbSet, Mapper);
    protected virtual GetById<TModel, TRequest, TResponse> GetByIdOperation => new(DbSet, Mapper);
    protected virtual Create<TModel, TRequest, TResponse> CreateOperation => new(DbContext, DbSet, Mapper);
    protected virtual Update<TModel, TRequest, TResponse> UpdateOperation => new(DbContext, DbSet, Mapper);
    protected virtual Delete<TModel> DeleteOperation => new(DbContext, DbSet);

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