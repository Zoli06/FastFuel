using AutoMapper;
using FastFuel.Features.Common;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Allergies.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AllergyController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllergies()
    {
        var allergies = context.Allergies.ToList();
        return Ok(mapper.Map<List<DTOs.AllergyDto>>(allergies));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetAllergy(uint id)
    {
        var allergy = context.Allergies.Find(id);
        if (allergy == null)
            return NotFound();
        return Ok(mapper.Map<DTOs.AllergyDto>(allergy));
    }

    [HttpPost]
    public IActionResult CreateAllergy(DTOs.EditAllergyDto allergyDto)
    {
        var allergy = mapper.Map<Models.Allergy>(allergyDto);
        // this is ugly
        allergy.Ingredients = context.Ingredients
            .Where(i => allergyDto.IngredientIds.Contains(i.Id))
            .ToList();
        context.Allergies.Add(allergy);
        context.SaveChanges();
        var createdAllergyDto = mapper.Map<DTOs.AllergyDto>(allergy);
        return CreatedAtAction(nameof(GetAllergy), new { id = allergy.Id }, createdAllergyDto);
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateAllergy(uint id, DTOs.EditAllergyDto allergyDto)
    {
        var allergy = context.Allergies.Find(id);
        if (allergy == null)
            return NotFound();
        allergy.Ingredients = context.Ingredients
            .Where(i => allergyDto.IngredientIds.Contains(i.Id))
            .ToList();
        mapper.Map(allergyDto, allergy);
        context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteAllergy(uint id)
    {
        var allergy = context.Allergies.Find(id);
        if (allergy == null)
            return NotFound();
        context.Allergies.Remove(allergy);
        context.SaveChanges();
        return NoContent();
    }
}