using FastFuel.Features.Common;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Foods.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FoodController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FoodResponseDto>>> GetFoods()
    {
        var foods = await context.Foods.ToListAsync();
        return Ok(foods.Select(f => f.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FoodResponseDto>> GetFood(uint id)
    {
        var food = await context.Foods.FindAsync(id);
        if (food == null)
            return NotFound();

        return Ok(food.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<FoodResponseDto>> CreateFood(FoodRequestDto foodRequestDto)
    {
        var food = foodRequestDto.ToModel();
        context.Foods.Add(food);
        await context.SaveChangesAsync();

        var createdFoodDto = food.ToDto();
        return CreatedAtAction(nameof(GetFood), new { id = food.Id }, createdFoodDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateFood(uint id, FoodRequestDto foodRequestDto)
    {
        var food = await context.Foods.FindAsync(id);
        if (food == null)
            return NotFound();

        food.UpdateModel(foodRequestDto);
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