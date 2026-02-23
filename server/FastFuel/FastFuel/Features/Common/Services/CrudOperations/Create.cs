using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services.CrudOperations;

public class Create<TModel, TRequest, TResponse>(
    ApplicationDbContext dbContext,
    DbSet<TModel> dbSet,
    IMapper<TModel, TRequest, TResponse> mapper)
    where TModel : class
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected readonly DbSet<TModel> DbSet = dbSet;
    protected readonly IMapper<TModel, TRequest, TResponse> Mapper = mapper;

    protected virtual Task<TModel> CreateModelAsync(TRequest requestDto)
    {
        return Task.FromResult(Mapper.ToModel(requestDto));
    }

    protected virtual async Task SaveModelAsync(TRequest requestDto, TModel model)
    {
        DbSet.Add(model);
        await DbContext.SaveChangesAsync();
    }

    protected virtual Task<TResponse> CreateDtoAsync(TRequest requestDto, TModel model)
    {
        return Task.FromResult(Mapper.ToDto(model));
    }

    public virtual async Task<TResponse> ExecuteAsync(TRequest requestDto)
    {
        var model = await CreateModelAsync(requestDto);
        await SaveModelAsync(requestDto, model);
        return await CreateDtoAsync(requestDto, model);
    }
}