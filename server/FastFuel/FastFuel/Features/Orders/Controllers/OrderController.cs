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
    /// <summary>
    /// Gets the orders of the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current user's orders.</returns>
    [HttpGet("my")]
    [PermissionCheck("ReadOwn")]
    public async Task<Results<Ok<List<OrderResponseDto>>, UnauthorizedHttpResult>> GetMyOrders(
        CancellationToken cancellationToken = default)
    {
        return TypedResults.Ok(
            await ((IOrderService)Service).GetOrdersForCurrentUserAsync(User, cancellationToken));
    }

    /// <summary>
    /// Gets all orders with optional filters.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching orders, or a bad request response if a filter is invalid.</returns>
    [HttpGet]
    [SwaggerQueryParam("status", typeof(OrderStatus))]
    [SwaggerQueryParam("restaurantId", typeof(uint))]
    [PermissionCheck(CrudOperation.Read)]
    public override async Task<Results<Ok<List<OrderResponseDto>>, BadRequest<ProblemDetails>, UnauthorizedHttpResult,
        ForbidHttpResult>> GetAll(
        CancellationToken cancellationToken = default)
    {
        var status = HttpContext.Request.Query["status"].ToString();
        var restaurantId = HttpContext.Request.Query["restaurantId"].ToString();

        if (filterParamsFactory.TryParse(status, restaurantId, out var filterParams))
            return TypedResults.Ok(
                await ((IOrderService)Service).GetAllOrdersWithFiltersAsync(filterParams, cancellationToken));

        return TypedResults.BadRequest(new ProblemDetails
        {
            Title = "Invalid filter parameter",
            Detail = "Some filters were invalid"
        });
    }

    /// <summary>
    /// Updates the status of an existing order.
    /// </summary>
    /// <param name="id">The identifier of the order to update.</param>
    /// <param name="orderStatus">The new order status.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content when the status update succeeds; otherwise an error response.</returns>
    [HttpPut("{id:int}/status")]
    [PermissionCheck("UpdateStatus")]
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