using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Orders.Controllers;

// TODO: Allow staff to update order status and mark as completed
public class OrderController(ICrudService<OrderRequestDto, OrderResponseDto> service)
    : CrudController<Order, OrderRequestDto, OrderResponseDto>(service)
{
    [HttpGet("my")]
    public async Task<Results<Ok<List<OrderResponseDto>>, UnauthorizedHttpResult>> GetMyOrders(
        CancellationToken cancellationToken = default)
    {
        var orders = await service.GetAllAsync(cancellationToken);
        return TypedResults.Ok(orders);
    }
}