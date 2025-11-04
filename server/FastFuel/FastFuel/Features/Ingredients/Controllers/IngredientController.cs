using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Ingredients.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class IngredientController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredients()
    {
        var ingredients = await context.Ingredients.ToListAsync();
        return Ok(mapper.Map<List<IngredientDto>>(ingredients));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<IngredientDto>> GetIngredient(int id)
    {
        var ingredient = await context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return NotFound();

        return Ok(mapper.Map<IngredientDto>(ingredient));
    }

    [HttpPost]
    public async Task<ActionResult<IngredientDto>> CreateIngredient(EditIngredientDto ingredientDto)
    {
        var ingredient = mapper.Map<Ingredient>(ingredientDto);
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();

        var createdIngredientDto = mapper.Map<IngredientDto>(ingredient);
        return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, createdIngredientDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateIngredient(int id, EditIngredientDto ingredientDto)
    {
        var ingredient = await context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return NotFound();

        mapper.Map(ingredientDto, ingredient);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteIngredient(int id)
    {
        var ingredient = await context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return NotFound();

        context.Ingredients.Remove(ingredient);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
