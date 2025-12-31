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

        var targetIngredients = dbContext.Ingredients
            .Where(i => dto.IngredientIds.Contains(i.Id))
            .ToList();

        // Remove ingredients not present in the DTO
        foreach (var rem in model.Ingredients.Where(i => !dto.IngredientIds.Contains(i.Id)).ToList())
            model.Ingredients.Remove(rem);

        // Add new ingredients that are missing
        var existingIds = model.Ingredients.Select(i => i.Id).ToHashSet();
        foreach (var ing in targetIngredients.Where(ing => !existingIds.Contains(ing.Id)))
            model.Ingredients.Add(ing);
    }
}