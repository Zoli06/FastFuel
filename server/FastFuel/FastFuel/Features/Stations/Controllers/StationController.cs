using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Stations.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class StationController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public IActionResult GeStStations()
    {
        var stations = context.Stations.ToList();
        return Ok(mapper.Map<List<StationDto>>(stations));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetStation(uint id)
    {
        var station = context.Stations.Find(id);
        if (station == null)
            return NotFound();
        return Ok(mapper.Map<StationDto>(station));
    }

    [HttpPost]
    public IActionResult CreateStation(EditStationDto stationDto)
    {
        var station = mapper.Map<Station>(stationDto);
        context.Stations.Add(station);
        context.SaveChanges();
        var createdDto = mapper.Map<StationDto>(station);
        return CreatedAtAction(nameof(GetStation), new { id = station.Id }, createdDto);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateStation(uint id, EditStationDto stationDto)
    {
        var station = context.Stations.Find(id);
        if (station == null)
            return NotFound();
        mapper.Map(stationDto, station);
        context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteStation(uint id)
    {
        var station = context.Stations.Find(id);
        if (station == null)
            return NotFound();
        context.Stations.Remove(station);
        context.SaveChanges();
        return NoContent();
    }
}