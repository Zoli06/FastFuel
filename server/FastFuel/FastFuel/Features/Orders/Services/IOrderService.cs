using System.Security.Claims;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Services.OrderFilter;

namespace FastFuel.Features.Orders.Services;

public interface IOrderService : ICrudService<OrderRequestDto, OrderResponseDto>
{
    Task<List<OrderResponseDto>> GetOrdersForCurrentUserAsync(ClaimsPrincipal user,
        CancellationToken cancellationToken = default);

    Task<List<OrderResponseDto>> GetAllOrdersWithFiltersAsync(IOrderFilterParams filterParams,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateOrderStatusAsync(uint orderId, OrderStatus newStatus,
        CancellationToken cancellationToken = default);
}