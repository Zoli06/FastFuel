using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Common.Services;

public interface ICrudService<in TRequest, TResponse> where TResponse : IIdentifiable
{
    Task<List<TResponse>> GetAllAsync(uint? userId = null, CancellationToken cancellationToken = default);
    Task<TResponse?> GetByIdAsync(uint id, uint? userId = null, CancellationToken cancellationToken = default);

    Task<TResponse> CreateAsync(TRequest requestDto, uint? userId = null,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(uint id, TRequest requestDto, uint? userId = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(uint id, uint? userId = null, CancellationToken cancellationToken = default);
}