using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services;

public abstract class CrudService<TModel, TRequest, TResponse>(
    ApplicationDbContext dbContext,
    IMapper<TModel, TRequest, TResponse> mapper)
    : ICrudService<TRequest, TResponse> where TModel : class, IIdentifiable where TResponse : IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected abstract DbSet<TModel> DbSet { get; }

    public virtual async Task<List<TResponse>> GetAllAsync()
    {
        await OnBeforeGetAllAsync();
        var models = await DbSet.ToListAsync();
        await OnAfterGetAllAsync(models);
        var dtos = models.ConvertAll(mapper.ToDto);
        var finalDtos = await OnTransformGetAllResultAsync(dtos);
        return finalDtos;
    }

    public virtual async Task<TResponse?> GetByIdAsync(uint id)
    {
        await OnBeforeGetByIdAsync(id);
        var model = await DbSet.FindAsync(id);
        if (model == null)
            return default;
        await OnAfterGetByIdAsync(model);
        var dto = mapper.ToDto(model);
        var finalDto = await OnTransformGetByIdResultAsync(dto);
        return finalDto;
    }

    public virtual async Task<TResponse> CreateAsync(TRequest requestDto)
    {
        await OnBeforeCreateAsync(requestDto);
        var model = mapper.ToModel(requestDto);
        await OnBeforeCreateModelAsync(model);
        DbSet.Add(model);
        await DbContext.SaveChangesAsync();
        await OnAfterCreateAsync(model);
        var responseDto = mapper.ToDto(model);
        var finalDto = await OnTransformCreateResultAsync(responseDto);
        return finalDto;
    }

    public virtual async Task<bool> DeleteAsync(uint id)
    {
        await OnBeforeDeleteAsync(id);
        var model = await DbSet.FindAsync(id);
        if (model == null)
            return false;
        await OnBeforeDeleteModelAsync(model);
        DbSet.Remove(model);
        await DbContext.SaveChangesAsync();
        await OnAfterDeleteAsync(model);
        return true;
    }

    public virtual async Task<bool> UpdateAsync(uint id, TRequest requestDto)
    {
        await OnBeforeUpdateAsync(id, requestDto);
        var model = await DbSet
            .FirstOrDefaultAsync(a => a.Id == id);

        if (model == null)
            return false;

        mapper.UpdateModel(requestDto, ref model);
        await OnBeforeUpdateModelAsync(model);
        await DbContext.SaveChangesAsync();
        await OnAfterUpdateAsync(model);
        return true;
    }

    protected virtual Task OnBeforeGetAllAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnAfterGetAllAsync(List<TModel> models)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<List<TResponse>> OnTransformGetAllResultAsync(List<TResponse> dtos)
    {
        return Task.FromResult(dtos);
    }

    protected virtual Task OnBeforeGetByIdAsync(uint id)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnAfterGetByIdAsync(TModel model)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<TResponse> OnTransformGetByIdResultAsync(TResponse dto)
    {
        return Task.FromResult(dto);
    }

    protected virtual Task OnBeforeCreateAsync(TRequest requestDto)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnBeforeCreateModelAsync(TModel model)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnAfterCreateAsync(TModel model)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<TResponse> OnTransformCreateResultAsync(TResponse dto)
    {
        return Task.FromResult(dto);
    }

    protected virtual Task OnBeforeUpdateAsync(uint id, TRequest requestDto)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnBeforeUpdateModelAsync(TModel model)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnAfterUpdateAsync(TModel model)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnBeforeDeleteAsync(uint id)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnBeforeDeleteModelAsync(TModel model)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnAfterDeleteAsync(TModel model)
    {
        return Task.CompletedTask;
    }
}