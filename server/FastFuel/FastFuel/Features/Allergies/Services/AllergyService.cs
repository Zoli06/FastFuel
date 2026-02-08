using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Allergies.Services;

public class AllergyService(
    ApplicationDbContext dbContext,
    IMapper<Allergy, AllergyRequestDto, AllergyResponseDto> mapper)
    : CrudService<Allergy, AllergyRequestDto, AllergyResponseDto>(dbContext, mapper)
{
    protected override DbSet<Allergy> DbSet { get; } = dbContext.Allergies;
}