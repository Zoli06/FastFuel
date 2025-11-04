using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Allergies.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AllergyController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AllergyDto>>> GetAllergies()
    {
        var allergies = await context.Allergies.ToListAsync();
        return Ok(mapper.Map<List<AllergyDto>>(allergies));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AllergyDto>> GetAllergy(int id)
    {
        var allergy = await context.Allergies.FindAsync(id);
        if (allergy == null)
            return NotFound();

        return Ok(mapper.Map<AllergyDto>(allergy));
    }

    [HttpPost]
    public async Task<ActionResult<AllergyDto>> CreateAllergy(EditAllergyDto allergyDto)
    {
        var allergy = mapper.Map<Allergy>(allergyDto);
        context.Allergies.Add(allergy);
        await context.SaveChangesAsync();

        var createdAllergyDto = mapper.Map<AllergyDto>(allergy);
        return CreatedAtAction(nameof(GetAllergy), new { id = allergy.Id }, createdAllergyDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAllergy(int id, EditAllergyDto allergyDto)
    {
        var allergy = await context.Allergies.FindAsync(id);
        if (allergy == null)
            return NotFound();

        mapper.Map(allergyDto, allergy);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAllergy(int id)
    {
        var allergy = await context.Allergies.FindAsync(id);
        if (allergy == null)
            return NotFound();

        context.Allergies.Remove(allergy);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
