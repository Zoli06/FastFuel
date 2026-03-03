using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class Create<TEntity, TRequest, TResponse>(
    ApplicationDbContext dbContext,
    DbSet<TEntity> dbSet,
    IMapper<TEntity, TRequest, TResponse> mapper)
    where TEntity : class
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly DbSet<TEntity> DbSet = dbSet;
    protected readonly IMapper<TEntity, TRequest, TResponse> Mapper = mapper;

    protected virtual Task<TEntity> CreateEntityAsync(TRequest requestDto, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Mapper.ToEntity(requestDto));
    }

    protected virtual async Task SaveEntityAsync(TRequest requestDto, TEntity entity, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        DbSet.Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    protected virtual Task<TResponse> CreateDtoAsync(TRequest requestDto, TEntity entity, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Mapper.ToDto(entity));
    }

    public virtual async Task<TResponse> ExecuteAsync(TRequest requestDto, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        var entity = await CreateEntityAsync(requestDto, userId, cancellationToken);
        await SaveEntityAsync(requestDto, entity, userId, cancellationToken);
        return await CreateDtoAsync(requestDto, entity, userId, cancellationToken);
    }
}