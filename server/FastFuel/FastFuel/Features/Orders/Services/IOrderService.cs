using System.Security.Claims;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Orders.DTOs;

namespace FastFuel.Features.Orders.Services;

public interface IOrderService : ICrudService<OrderRequestDto, OrderResponseDto>
{
    Task<List<OrderResponseDto>> GetOrdersForCurrentUserAsync(ClaimsPrincipal user,
        CancellationToken cancellationToken = default);
}