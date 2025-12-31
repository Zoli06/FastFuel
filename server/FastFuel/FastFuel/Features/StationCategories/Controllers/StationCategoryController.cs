using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.StationCategories.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class StationCategoryController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public IActionResult GetStationCategories()
    {
        var categories = context.StationCategories.ToList();
        return Ok(mapper.Map<List<StationCategoryDto>>(categories));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetStationCategory(uint id)
    {
        var category = context.StationCategories.Find(id);
        if (category == null)
            return NotFound();
        return Ok(mapper.Map<StationCategoryDto>(category));
    }

    [HttpPost]
    public IActionResult CreateStationCategory(EditStationCategoryDto categoryDto)
    {
        var category = mapper.Map<StationCategory>(categoryDto);
        //TODO: I hate this, switch to Mapperly
        category.Ingredients = context.Ingredients
            .Where(i => categoryDto.IngredientIds.Contains(i.Id))
            .ToList();
        category.Stations = context.Stations
            .Where(s => categoryDto.StationIds.Contains(s.Id))
            .ToList();
        context.StationCategories.Add(category);
        context.SaveChanges();
        var createdCategoryDto = mapper.Map<StationCategoryDto>(category);
        return CreatedAtAction(nameof(GetStationCategory), new { id = category.Id }, createdCategoryDto);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateStationCategory(uint id, EditStationCategoryDto categoryDto)
    {
        var category = context.StationCategories.Find(id);
        if (category == null)
            return NotFound();
        category.Ingredients = context.Ingredients
            .Where(i => categoryDto.IngredientIds.Contains(i.Id))
            .ToList();
        category.Stations = context.Stations
            .Where(s => categoryDto.StationIds.Contains(s.Id))
            .ToList();
        mapper.Map(categoryDto, category);
        context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteStationCategory(uint id)
    {
        var category = context.StationCategories.Find(id);
        if (category == null)
            return NotFound();
        context.StationCategories.Remove(category);
        context.SaveChanges();
        return NoContent();
    }
}