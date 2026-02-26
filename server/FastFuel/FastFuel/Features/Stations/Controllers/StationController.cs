using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Entities;
using FastFuel.Features.Stations.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Stations.Controllers;

public class StationController(IStationService service)
    : CrudController<Station, StationRequestDto, StationResponseDto>(service)
{
    private IStationService TasksService { get; } = service;

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