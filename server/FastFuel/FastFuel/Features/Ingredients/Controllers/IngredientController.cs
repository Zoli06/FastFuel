using FastFuel.Features.Common;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Ingredients.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class IngredientController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientResponseDto>>> GetIngredients()
    {
        var ingredients = await context.Ingredients.ToListAsync();
        return Ok(ingredients.Select(i => i.ToDto()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IngredientResponseDto>> GetIngredient(uint id)
    {
        var ingredient = await context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return NotFound();
        return Ok(ingredient.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<IngredientResponseDto>> CreateIngredient(IngredientRequestDto ingredientRequestDto)
    {
        var ingredient = ingredientRequestDto.ToModel(context);
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();
        var createdDto = ingredient.ToDto();
        return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, createdDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateIngredient(uint id, IngredientRequestDto ingredientRequestDto)
    {
        var ingredient = await context.Ingredients
            .FirstOrDefaultAsync(i => i.Id == id);

        if (ingredient == null)
            return NotFound();

        ingredient.UpdateModel(ingredientRequestDto, context);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteIngredient(uint id)
    {
        var ingredient = await context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return NotFound();
        context.Ingredients.Remove(ingredient);
        await context.SaveChangesAsync();
        return NoContent();
    }
}