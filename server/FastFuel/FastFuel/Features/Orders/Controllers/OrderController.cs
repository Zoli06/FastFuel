using FastFuel.Features.Common.Authorization;
using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.Orders.Services;
using FastFuel.NSwag.SwaggerQueryParam;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Orders.Controllers;

// TODO: Allow staff to update order status and mark as completed
public class OrderController(IOrderService service, IOrderFilterParamsFactory filterParamsFactory)
    : CrudController<Order, OrderRequestDto, OrderResponseDto>(service)
{
    [HttpGet("my")]
    public async Task<Results<Ok<List<OrderResponseDto>>, UnauthorizedHttpResult>> GetMyOrders(
        CancellationToken cancellationToken = default)
    {
        var orders = await Service.GetAllAsync(cancellationToken);
        return TypedResults.Ok(orders);
    }

    // I'm sorry...
    [SwaggerQueryParam("status", typeof(OrderStatus))]
    [CrudAuthorize(PermissionType.Read)]
    public override async Task<Results<Ok<List<OrderResponseDto>>, BadRequest<ProblemDetails>, UnauthorizedHttpResult,
        ForbidHttpResult>> GetAll(
        CancellationToken cancellationToken = default)
    {
        var status = HttpContext.Request.Query["status"].ToString();

        if (filterParamsFactory.TryParse(status, out var filterParams))
            return TypedResults.Ok(
                await ((IOrderService)Service).GetAllOrdersWithFiltersAsync(filterParams, cancellationToken));

        return TypedResults.BadRequest(new ProblemDetails
        {
            Title = "Invalid filter parameter",
            Detail = "Some filters were invalid"
        });
    }
}