using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Common.Services;

public interface ICrudService<in TRequest, TResponse> where TResponse : IIdentifiable
{
    Task<List<TResponse>> GetAllAsync();
    Task<TResponse?> GetByIdAsync(uint id);
    Task<TResponse> CreateAsync(TRequest requestDto);
    Task<bool> UpdateAsync(uint id, TRequest requestDto);
    Task<bool> DeleteAsync(uint id);
}