using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.OpeningHours.DTOs;
using FastFuel.Features.OpeningHours.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.OpeningHours.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class OpeningHourController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OpeningHourDto>>> GetOpeningHours()
    {
        var openingHours = await context.OpeningHours.ToListAsync();
        return Ok(mapper.Map<List<OpeningHourDto>>(openingHours));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OpeningHourDto>> GetOpeningHour(int id)
    {
        var openingHour = await context.OpeningHours.FindAsync(id);
        if (openingHour == null)
            return NotFound();

        return Ok(mapper.Map<OpeningHourDto>(openingHour));
    }

    [HttpPost]
    public async Task<ActionResult<OpeningHourDto>> CreateOpeningHour(EditOpeningHourDto openingHourDto)
    {
        var openingHour = mapper.Map<OpeningHour>(openingHourDto);
        context.OpeningHours.Add(openingHour);
        await context.SaveChangesAsync();

        var createdOpeningHourDto = mapper.Map<OpeningHourDto>(openingHour);
        return CreatedAtAction(nameof(GetOpeningHour), new { id = openingHour.Id }, createdOpeningHourDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOpeningHour(int id, EditOpeningHourDto openingHourDto)
    {
        var openingHour = await context.OpeningHours.FindAsync(id);
        if (openingHour == null)
            return NotFound();

        mapper.Map(openingHourDto, openingHour);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOpeningHour(int id)
    {
        var openingHour = await context.OpeningHours.FindAsync(id);
        if (openingHour == null)
            return NotFound();

        context.OpeningHours.Remove(openingHour);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
