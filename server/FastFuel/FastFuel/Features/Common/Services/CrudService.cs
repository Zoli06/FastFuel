using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Mappers;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Services;

public abstract class CrudService<TModel, TRequest, TResponse>(ApplicationDbContext dbContext) where TModel : class, IIdentifiable where TResponse : IIdentifiable
{
    protected readonly ApplicationDbContext DbContext = dbContext;
    protected abstract Mapper<TModel, TRequest, TResponse> Mapper { get; }
    protected abstract DbSet<TModel> DbSet { get; }

    public virtual async Task<List<TResponse>> GetAllAsync()
    {
        await OnBeforeGetAllAsync();
        var models = await DbSet.ToListAsync();
        await OnAfterGetAllAsync(models);
        var dtos = models.ConvertAll(m => Mapper.ToDto(m));
        var finalDtos = await OnTransformGetAllResultAsync(dtos);
        return finalDtos;
    }
    
    protected virtual Task OnBeforeGetAllAsync() => Task.CompletedTask;
    protected virtual Task OnAfterGetAllAsync(List<TModel> models) => Task.CompletedTask;
    protected virtual Task<List<TResponse>> OnTransformGetAllResultAsync(List<TResponse> dtos) => Task.FromResult(dtos);
    
    public virtual async Task<TResponse?> GetByIdAsync(uint id)
    {
        await OnBeforeGetByIdAsync(id);
        var model = await DbSet.FindAsync(id);
        if (model == null)
            return default;
        await OnAfterGetByIdAsync(model);
        var dto = Mapper.ToDto(model);
        var finalDto = await OnTransformGetByIdResultAsync(dto);
        return finalDto;
    }
    
    protected virtual Task OnBeforeGetByIdAsync(uint id) => Task.CompletedTask;
    protected virtual Task OnAfterGetByIdAsync(TModel model) => Task.CompletedTask;
    protected virtual Task<TResponse> OnTransformGetByIdResultAsync(TResponse dto) => Task.FromResult(dto);
    
    public virtual async Task<TResponse> CreateAsync(TRequest requestDto)
    {
        await OnBeforeCreateAsync(requestDto);
        var model = Mapper.ToModel(requestDto);
        await OnBeforeCreateModelAsync(model);
        DbSet.Add(model);
        await DbContext.SaveChangesAsync();
        await OnAfterCreateAsync(model);
        var responseDto = Mapper.ToDto(model);
        var finalDto = await OnTransformCreateResultAsync(responseDto);
        return finalDto;
    }
    
    protected virtual Task OnBeforeCreateAsync(TRequest requestDto) => Task.CompletedTask;
    protected virtual Task OnBeforeCreateModelAsync(TModel model) => Task.CompletedTask;
    protected virtual Task OnAfterCreateAsync(TModel model) => Task.CompletedTask;
    protected virtual Task<TResponse> OnTransformCreateResultAsync(TResponse dto) => Task.FromResult(dto);
    
    public virtual async Task<bool> UpdateAsync(uint id, TRequest requestDto)
    {
        await OnBeforeUpdateAsync(id, requestDto);
        var model = await DbSet
            .FirstOrDefaultAsync(a => a.Id == id);

        if (model == null)
            return false;

        Mapper.UpdateModel(requestDto, ref model);
        await OnBeforeUpdateModelAsync(model);
        await DbContext.SaveChangesAsync();
        await OnAfterUpdateAsync(model);
        return true;
    }
    
    protected virtual Task OnBeforeUpdateAsync(uint id, TRequest requestDto) => Task.CompletedTask;
    protected virtual Task OnBeforeUpdateModelAsync(TModel model) => Task.CompletedTask;
    protected virtual Task OnAfterUpdateAsync(TModel model) => Task.CompletedTask;
    
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
    
    protected virtual Task OnBeforeDeleteAsync(uint id) => Task.CompletedTask;
    protected virtual Task OnBeforeDeleteModelAsync(TModel model) => Task.CompletedTask;
    protected virtual Task OnAfterDeleteAsync(TModel model) => Task.CompletedTask;
}