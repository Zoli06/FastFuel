using System.Security.Claims;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.DTOs;

namespace FastFuel.Features.Orders.Services;

public interface IOrderService
{
    Task<List<OrderResponseDto>> GetAllAsync(uint? userId = null, CancellationToken cancellationToken = default);
    Task<OrderResponseDto?> GetByIdAsync(uint id, uint? userId = null, CancellationToken cancellationToken = default);

    Task<OrderResponseDto> CreateAsync(OrderRequestDto requestDto, uint? userId = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(uint id, uint? userId = null, CancellationToken cancellationToken = default);

    Task<List<OrderResponseDto>> GetOrdersForCurrentUserAsync(ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    Task<List<OrderResponseDto>> GetAllOrdersWithFiltersAsync(OrderFilterParams filterParams,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateOrderStatusAsync(uint orderId, OrderStatus newStatus,
        CancellationToken cancellationToken = default);
}