using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Allergies.Services;
using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;

namespace FastFuel.Features.Allergies.Controllers;

public class AllergyController(ApplicationDbContext dbContext)
    : CrudController<Allergy, AllergyRequestDto, AllergyResponseDto>
{
    protected override CrudService<Allergy, AllergyRequestDto, AllergyResponseDto> Service { get; } = new AllergyService(dbContext);
}