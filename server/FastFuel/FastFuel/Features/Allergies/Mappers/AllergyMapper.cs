using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Entities;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Allergies.Mappers;

public class AllergyMapper(FastFuelDbContext dbContext)
    : IMapper<Allergy, AllergyRequestDto, AllergyResponseDto>
{
    public AllergyResponseDto ToDto(Allergy entity)
    {
        return new AllergyResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IngredientIds = entity.Ingredients.ConvertAll(i => i.Id)
        };
    }

    public Allergy ToEntity(AllergyRequestDto dto)
    {
        return new Allergy
        {
            Name = dto.Name,
            Ingredients = dbContext.Ingredients
                .Where(i => dto.IngredientIds.Contains(i.Id))
                .ToList()
        };
    }


    public void UpdateEntity(AllergyRequestDto dto, Allergy entity)
    {
        entity.Name = dto.Name;

        entity.Ingredients.Clear();
        entity.Ingredients.AddRange(dbContext.Ingredients
            .Where(i => dto.IngredientIds.Contains(i.Id))
            .ToList());
    }
}