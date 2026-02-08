using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;

namespace FastFuel.Features.Allergies.Controllers;

public class AllergyController(ICrudService<AllergyRequestDto, AllergyResponseDto> service)
    : CrudController<Allergy, AllergyRequestDto, AllergyResponseDto>(service);