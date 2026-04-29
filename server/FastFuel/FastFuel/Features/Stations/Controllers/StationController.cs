using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Entities;
using FastFuel.Features.Stations.Services;
using FastFuel.NSwag.SwaggerQueryParam;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Stations.Controllers;

public class StationController(IStationService service)
    : CrudController<Station, StationRequestDto, StationResponseDto>(service)
{
    private IStationService TasksService { get; } = service;

    /// <summary>
    /// Gets all stations with optional filters.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The matching stations, or a bad request response if a filter is invalid.</returns>
    [HttpGet]
    [SwaggerQueryParam("restaurantId", typeof(uint))]
    [PermissionCheck(CrudOperation.Read)]
    public override async Task<Results<
            Ok<List<StationResponseDto>>,
            BadRequest<ProblemDetails>,
            UnauthorizedHttpResult,
            ForbidHttpResult>>
        GetAll(CancellationToken cancellationToken = default)
    {
        var restaurantId = HttpContext.Request.Query["restaurantId"].ToString();
        uint parsedRestaurantId = 0;

        if (!string.IsNullOrEmpty(restaurantId) && !uint.TryParse(restaurantId, out parsedRestaurantId))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid filter parameter",
                Detail = "Some filters were invalid"
            });

        uint? restaurantIdFilter = string.IsNullOrEmpty(restaurantId) ? null : parsedRestaurantId;

        return TypedResults.Ok(
            await TasksService.GetAllStationsWithFiltersAsync(restaurantIdFilter, cancellationToken));
    }

    /// <summary>
    /// Gets the task list for a station.
    /// </summary>
    /// <param name="id">The identifier of the station.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The station tasks when the station exists.</returns>
    [PermissionCheck("ViewTasks")]
    [HttpGet("{id:int}/tasks")]
    public async Task<ActionResult<List<StationTasksResponseDto>>> GetTasks(uint id,
        CancellationToken cancellationToken = default)
    {
        var tasks = await TasksService.GetTasksForStationAsync(id, cancellationToken);
        if (tasks == null)
            return NotFound();
        return Ok(tasks);
    }
}