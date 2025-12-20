using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common;

namespace FastFuel.Features.Allergies.Mappers;

public static class AllergyMapper
{
    public static AllergyResponseDto ToDto(this Allergy allergy)
    {
        return new AllergyResponseDto
        {
            Id = allergy.Id,
            Name = allergy.Name,
            Message = allergy.Message,
            IngredientIds = allergy.Ingredients.ConvertAll(i => i.Id)
        };
    }

    public static Allergy ToModel(this AllergyRequestDto requestDto, ApplicationDbContext dbContext)
    {
        return new Allergy
        {
            Name = requestDto.Name,
            Message = requestDto.Message,
            // TODO: Ask Timi whether is this good practice
            // The other alternative is to load Ingredients in the service layer
            Ingredients = dbContext.Ingredients
                .Where(i => requestDto.IngredientIds.Contains(i.Id))
                .ToList()
        };
    }


    public static void UpdateModel(this Allergy allergy, AllergyRequestDto requestDto, ApplicationDbContext dbContext)
    {
        allergy.Name = requestDto.Name;
        allergy.Message = requestDto.Message;

        var targetIngredients = dbContext.Ingredients
            .Where(i => requestDto.IngredientIds.Contains(i.Id))
            .ToList();

        // Remove ingredients not present in the DTO
        foreach (var rem in allergy.Ingredients.Where(i => !requestDto.IngredientIds.Contains(i.Id)).ToList())
            allergy.Ingredients.Remove(rem);

        // Add new ingredients that are missing
        var existingIds = allergy.Ingredients.Select(i => i.Id).ToHashSet();
        foreach (var ing in targetIngredients.Where(ing => !existingIds.Contains(ing.Id)))
            allergy.Ingredients.Add(ing);
    }
}