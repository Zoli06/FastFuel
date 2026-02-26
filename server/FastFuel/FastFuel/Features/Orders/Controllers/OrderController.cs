using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.Orders.Services;
using FastFuel.Features.Orders.Services.OrderFilter;
using FastFuel.NSwag.SwaggerQueryParam;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Orders.Controllers;

public class OrderController(IOrderService service, IOrderFilterParamsFactory filterParamsFactory)
    : CrudController<Order, OrderRequestDto, OrderResponseDto>(service)
{
    [HttpGet("my")]
    public async Task<Results<Ok<List<OrderResponseDto>>, UnauthorizedHttpResult>> GetMyOrders(
        CancellationToken cancellationToken = default)
    {
        return TypedResults.Ok(
            await ((IOrderService)Service).GetOrdersForCurrentUserAsync(User, cancellationToken));
    }

    [SwaggerQueryParam("status", typeof(OrderStatus))]
    [PermissionCheck(CrudOperation.Read)]
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

    [HttpPut("{id:int}/status")]
    [PermissionCheck(CrudOperation.Update)]
    public async Task<Results<
            NoContent,
            NotFound,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        UpdateOrderStatus(
            uint id,
            [FromBody] OrderStatus orderStatus,
            CancellationToken cancellationToken = default
        )
    {
        var success = await ((IOrderService)Service).UpdateOrderStatusAsync(id, orderStatus, cancellationToken);
        if (success)
            return TypedResults.NoContent();

        return TypedResults.NotFound();
    }
}