using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Allergies.Mappers;

public class AllergyMapper(ApplicationDbContext dbContext)
    : IMapper<Allergy, AllergyRequestDto, AllergyResponseDto>
{
    public AllergyResponseDto ToDto(Allergy model)
    {
        return new AllergyResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Message = model.Message,
            IngredientIds = model.Ingredients.ConvertAll(i => i.Id)
        };
    }

    public Allergy ToModel(AllergyRequestDto dto)
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


    public void UpdateModel(AllergyRequestDto dto, ref Allergy model)
    {
        model.Name = dto.Name;
        model.Message = dto.Message;

        model.Ingredients.Clear();
        model.Ingredients.AddRange(dbContext.Ingredients
            .Where(i => dto.IngredientIds.Contains(i.Id))
            .ToList());
    }
}