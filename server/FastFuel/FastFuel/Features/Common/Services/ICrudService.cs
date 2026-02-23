using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Common.Services;

public interface ICrudService<in TRequest, TResponse> where TResponse : IIdentifiable
{
    Task<List<TResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TResponse?> GetByIdAsync(uint id, CancellationToken cancellationToken = default);
    Task<TResponse> CreateAsync(TRequest requestDto, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(uint id, TRequest requestDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(uint id, CancellationToken cancellationToken = default);
}