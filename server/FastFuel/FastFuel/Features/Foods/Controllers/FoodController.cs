using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Foods.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FoodController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FoodDto>>> GetFoods()
    {
        var foods = await context.Foods.ToListAsync();
        return Ok(mapper.Map<List<FoodDto>>(foods));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FoodDto>> GetFood(uint id)
    {
        var food = await context.Foods.FindAsync(id);
        if (food == null)
            return NotFound();

        return Ok(mapper.Map<FoodDto>(food));
    }

    [HttpPost]
    public async Task<ActionResult<FoodDto>> CreateFood(EditFoodDto foodDto)
    {
        var food = mapper.Map<Food>(foodDto);
        context.Foods.Add(food);
        await context.SaveChangesAsync();

        var createdFoodDto = mapper.Map<FoodDto>(food);
        return CreatedAtAction(nameof(GetFood), new { id = food.Id }, createdFoodDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateFood(uint id, EditFoodDto foodDto)
    {
        var food = await context.Foods.FindAsync(id);
        if (food == null)
            return NotFound();

        mapper.Map(foodDto, food);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteFood(uint id)
    {
        var food = await context.Foods.FindAsync(id);
        if (food == null)
            return NotFound();

        context.Foods.Remove(food);
        await context.SaveChangesAsync();

        return NoContent();
    }
}