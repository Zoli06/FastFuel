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

    protected virtual Task<TEntity> CreateEntityAsync(TRequest requestDto)
    {
        return Task.FromResult(Mapper.ToEntity(requestDto));
    }

    protected virtual async Task SaveEntityAsync(TRequest requestDto, TEntity entity)
    {
        DbSet.Add(entity);
        await DbContext.SaveChangesAsync();
    }

    protected virtual Task<TResponse> CreateDtoAsync(TRequest requestDto, TEntity entity)
    {
        return Task.FromResult(Mapper.ToDto(entity));
    }

    public virtual async Task<TResponse> ExecuteAsync(TRequest requestDto)
    {
        var entity = await CreateEntityAsync(requestDto);
        await SaveEntityAsync(requestDto, entity);
        return await CreateDtoAsync(requestDto, entity);
    }
}