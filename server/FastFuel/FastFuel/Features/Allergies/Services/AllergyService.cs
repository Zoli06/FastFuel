using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Mappers;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Allergies.Services;

public class AllergyService(ApplicationDbContext dbContext) : CrudService<Allergy, AllergyRequestDto, AllergyResponseDto>(dbContext)
{
    protected override Mapper<Allergy, AllergyRequestDto, AllergyResponseDto> Mapper => new AllergyMapper(DbContext);
    protected override DbSet<Allergy> DbSet => DbContext.Allergies;
}