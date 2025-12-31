using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Mappers;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Allergies.Controllers;

public class AllergyController(ApplicationDbContext dbContext)
    : ApplicationController<Allergy, AllergyRequestDto, AllergyResponseDto>(dbContext)
{
    protected override Mapper<Allergy, AllergyRequestDto, AllergyResponseDto> Mapper => new AllergyMapper(DbContext);
    protected override DbSet<Allergy> DbSet => DbContext.Allergies;
}