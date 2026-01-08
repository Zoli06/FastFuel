using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common;

namespace FastFuel.Features.Allergies.Mappers;

public class AllergyMapper(ApplicationDbContext dbContext)
    : Mapper<Allergy, AllergyRequestDto, AllergyResponseDto>
{
    public override AllergyResponseDto ToDto(Allergy model)
    {
        return new AllergyResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Message = model.Message,
            IngredientIds = model.Ingredients.ConvertAll(i => i.Id)
        };
    }

    public override Allergy ToModel(AllergyRequestDto dto)
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


    public override void UpdateModel(AllergyRequestDto dto, ref Allergy model)
    {
        model.Name = dto.Name;
        model.Message = dto.Message;

        model.Ingredients.Clear();
        model.Ingredients.AddRange(dbContext.Ingredients
            .Where(i => dto.IngredientIds.Contains(i.Id))
            .ToList());
    }
}