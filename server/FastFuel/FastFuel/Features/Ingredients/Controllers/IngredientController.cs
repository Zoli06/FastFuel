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
    public async Task<ActionResult<IngredientDto>> GetIngredient(uint id)
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
        // TODO: Throw error on unknown id
        ingredient.Allergies = await context.Allergies
            .Where(a => ingredientDto.AllergyIds.Contains(a.Id))
            .ToListAsync();
        ingredient.StationCategories = await context.StationCategories
            .Where(sc => ingredientDto.StationCategoryIds.Contains(sc.Id))
            .ToListAsync();
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();
        var createdDto = mapper.Map<IngredientDto>(ingredient);
        return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, createdDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateIngredient(uint id, EditIngredientDto ingredientDto)
    {
        // Load the tracked entity including its navigation collections so EF Core can manage the join table correctly
        var ingredient = await context.Ingredients
            .Include(i => i.Allergies)
            .Include(i => i.StationCategories)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ingredient == null)
            return NotFound();
        ingredient.Allergies = await context.Allergies
            .Where(a => ingredientDto.AllergyIds.Contains(a.Id))
            .ToListAsync();
        ingredient.StationCategories = await context.StationCategories
            .Where(sc => ingredientDto.StationCategoryIds.Contains(sc.Id))
            .ToListAsync();
        mapper.Map(ingredientDto, ingredient);
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