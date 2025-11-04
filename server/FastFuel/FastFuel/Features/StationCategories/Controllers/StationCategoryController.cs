using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.StationCategories.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class StationCategoryController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationCategoryDto>>> GetStationCategories()
    {
        var categories = await context.StationCategories.ToListAsync();
        return Ok(mapper.Map<List<StationCategoryDto>>(categories));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StationCategoryDto>> GetStationCategory(int id)
    {
        var category = await context.StationCategories.FindAsync(id);
        if (category == null)
            return NotFound();

        return Ok(mapper.Map<StationCategoryDto>(category));
    }

    [HttpPost]
    public async Task<ActionResult<StationCategoryDto>> CreateStationCategory(EditStationCategoryDto categoryDto)
    {
        var category = mapper.Map<StationCategory>(categoryDto);
        context.StationCategories.Add(category);
        await context.SaveChangesAsync();

        var createdCategoryDto = mapper.Map<StationCategoryDto>(category);
        return CreatedAtAction(nameof(GetStationCategory), new { id = category.Id }, createdCategoryDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateStationCategory(int id, EditStationCategoryDto categoryDto)
    {
        var category = await context.StationCategories.FindAsync(id);
        if (category == null)
            return NotFound();

        mapper.Map(categoryDto, category);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteStationCategory(int id)
    {
        var category = await context.StationCategories.FindAsync(id);
        if (category == null)
            return NotFound();

        context.StationCategories.Remove(category);
        await context.SaveChangesAsync();

        return NoContent();
    }
}

