using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Stations.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class StationController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationDto>>> GetStations()
    {
        var stations = await context.Stations.ToListAsync();
        return Ok(mapper.Map<List<StationDto>>(stations));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StationDto>> GetStation(int id)
    {
        var station = await context.Stations.FindAsync(id);
        if (station == null)
            return NotFound();

        return Ok(mapper.Map<StationDto>(station));
    }

    [HttpPost]
    public async Task<ActionResult<StationDto>> CreateStation(EditStationDto stationDto)
    {
        var station = mapper.Map<Station>(stationDto);
        context.Stations.Add(station);
        await context.SaveChangesAsync();

        var createdStationDto = mapper.Map<StationDto>(station);
        return CreatedAtAction(nameof(GetStation), new { id = station.Id }, createdStationDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStation(int id, EditStationDto stationDto)
    {
        var station = await context.Stations.FindAsync(id);
        if (station == null)
            return NotFound();

        mapper.Map(stationDto, station);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStation(int id)
    {
        var station = await context.Stations.FindAsync(id);
        if (station == null)
            return NotFound();

        context.Stations.Remove(station);
        await context.SaveChangesAsync();

        return NoContent();
    }
}

