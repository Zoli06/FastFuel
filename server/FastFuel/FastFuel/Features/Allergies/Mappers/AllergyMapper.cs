using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Entities;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Allergies.Mappers;

public class AllergyMapper(ApplicationDbContext dbContext)
    : IMapper<Allergy, AllergyRequestDto, AllergyResponseDto>
{
    public AllergyResponseDto ToDto(Allergy entity)
    {
        return new AllergyResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Message = entity.Message,
            IngredientIds = entity.Ingredients.ConvertAll(i => i.Id)
        };
    }

    public Allergy ToEntity(AllergyRequestDto dto)
    {
        return new Allergy
        {
            Name = dto.Name,
            Message = dto.Message,
            // TODO: Ask Timi whether is this good practice
            // The other alternative is to load Ingredients in the service layer
            Ingredients = dbContext.Ingredients
                .Where(i => dto.IngredientIds.Contains(i.Id))
                .ToList()
        };
    }


    public void UpdateEntity(AllergyRequestDto dto, Allergy entity)
    {
        entity.Name = dto.Name;
        entity.Message = dto.Message;

        entity.Ingredients.Clear();
        entity.Ingredients.AddRange(dbContext.Ingredients
            .Where(i => dto.IngredientIds.Contains(i.Id))
            .ToList());
    }
}