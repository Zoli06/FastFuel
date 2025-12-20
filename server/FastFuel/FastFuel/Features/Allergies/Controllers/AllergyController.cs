using FastFuel.Features.Allergies.Mappers;
using FastFuel.Features.Common;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Allergies.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AllergyController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllergies()
    {
        var allergies = context.Allergies.ToList();
        return Ok(allergies.Select(a => a.ToDto()));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetAllergy(uint id)
    {
        var allergy = context.Allergies.Find(id);
        if (allergy == null)
            return NotFound();
        return Ok(allergy.ToDto());
    }
    
    [HttpPost]
    public IActionResult CreateAllergy(DTOs.AllergyRequestDto allergyRequestDto)
    {
        var allergy = allergyRequestDto.ToModel(context);
        context.Allergies.Add(allergy);
        context.SaveChanges();
        var createdDto = allergy.ToDto();
        return CreatedAtAction(nameof(GetAllergy), new { id = allergy.Id }, createdDto);
    }
    
    [HttpPut("{id:int}")]
    public IActionResult UpdateAllergy(uint id, DTOs.AllergyRequestDto allergyRequestDto)
    {
        var allergy = context.Allergies
            .FirstOrDefault(a => a.Id == id);
        
        if (allergy == null)
            return NotFound();
        
        allergy.UpdateModel(allergyRequestDto, context);
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