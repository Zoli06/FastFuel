using System.Security.Claims;
using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Orders.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService service)
    : ControllerBase
{
    /// <summary>
    ///     Gets all orders with optional filters.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="status">An optional order status filter.</param>
    /// <param name="restaurantId">An optional restaurant ID filter.</param>
    /// <returns>The matching orders, or a bad request response if a filter is invalid.</returns>
    [HttpGet]
    [PermissionCheck(CrudOperation.Read)]
    public async Task<Results<Ok<List<OrderResponseDto>>, BadRequest<ProblemDetails>, UnauthorizedHttpResult,
        ForbidHttpResult>> GetAll(
        OrderStatus? status,
        uint? restaurantId,
        CancellationToken cancellationToken = default)
    {
        var filterParams = new OrderFilterParams
        {
            Status = status,
            RestaurantId = restaurantId
        };

        return TypedResults.Ok(
            await service.GetAllOrdersWithFiltersAsync(filterParams, cancellationToken));
    }

    /// <summary>
    ///     Gets one entity.
    /// </summary>
    /// <param name="id">The entity id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity.</returns>
    [HttpGet("{id:int}")]
    [PermissionCheck(CrudOperation.Read)]
    public virtual async Task<Results<
            Ok<OrderResponseDto>,
            NotFound,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        GetById(uint id, CancellationToken cancellationToken = default)
    {
        var dto = await service.GetByIdAsync(id, GetUserId(User), cancellationToken);
        if (dto == null)
            return TypedResults.NotFound();
        return TypedResults.Ok(dto);
    }

    /// <summary>
    ///     Makes a new entity.
    /// </summary>
    /// <param name="requestDto">The entity data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created entity.</returns>
    [HttpPost]
    [PermissionCheck(CrudOperation.Create)]
    public virtual async Task<Results<
            Created<OrderResponseDto>,
            Conflict<ProblemDetails>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        Create(OrderRequestDto requestDto, CancellationToken cancellationToken = default)
    {
        var responseDto = await service.CreateAsync(requestDto, GetUserId(User), cancellationToken);
        var location = Url.Action(nameof(GetById), new { id = responseDto.Id });
        return TypedResults.Created(location!, responseDto);
    }

    /// <summary>
    ///     Deletes one entity.
    /// </summary>
    /// <param name="id">The entity id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:int}")]
    [PermissionCheck(CrudOperation.Delete)]
    public virtual async Task<Results<
            NoContent,
            NotFound,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        Delete(uint id, CancellationToken cancellationToken = default)
    {
        var success = await service.DeleteAsync(id, GetUserId(User), cancellationToken);
        if (!success)
            return TypedResults.NotFound();
        return TypedResults.NoContent();
    }

    private uint? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim != null && uint.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }

    /// <summary>
    ///     Gets the orders of the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current user's orders.</returns>
    [HttpGet("my")]
    public async Task<Results<Ok<List<OrderResponseDto>>, UnauthorizedHttpResult>> GetMyOrders(
        CancellationToken cancellationToken = default)
    {
        return TypedResults.Ok(
            await service.GetOrdersForCurrentUserAsync(User, cancellationToken));
    }

    /// <summary>
    ///     Updates the status of an existing order.
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
        var success = await service.UpdateOrderStatusAsync(id, orderStatus, cancellationToken);
        if (success)
            return TypedResults.NoContent();

        return TypedResults.NotFound();
    }
}