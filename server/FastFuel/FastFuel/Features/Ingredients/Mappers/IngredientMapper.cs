using FastFuel.Features.Common;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Models;

namespace FastFuel.Features.Ingredients.Mappers;

public class IngredientMapper(ApplicationDbContext dbContext)
    : Mapper<Ingredient, IngredientRequestDto, IngredientResponseDto>
{
    public override IngredientResponseDto ToDto(Ingredient model)
    {
        return new IngredientResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            ImageUrl = model.ImageUrl,
            AllergyIds = model.Allergies.ConvertAll(allergy => allergy.Id),
            StationCategoryIds = model.StationCategories.ConvertAll(category => category.Id),
            FoodIds = model.FoodIngredients.ConvertAll(fi => fi.FoodId)
        };
    }

    public override Ingredient ToModel(IngredientRequestDto dto)
    {
        return new Ingredient
        {
            Name = dto.Name,
            ImageUrl = dto.ImageUrl,
            Allergies = dbContext.Allergies
                .Where(a => dto.AllergyIds.Contains(a.Id))
                .ToList(),
            StationCategories = dbContext.StationCategories
                .Where(sc => dto.StationCategoryIds.Contains(sc.Id))
                .ToList()
        };
    }

    public override void UpdateModel(IngredientRequestDto dto, ref Ingredient model)
    {
        model.Name = dto.Name;
        model.ImageUrl = dto.ImageUrl;

        var targetAllergies = dbContext.Allergies
            .Where(a => dto.AllergyIds.Contains(a.Id))
            .ToList();

        // Remove allergies not present in the DTO
        foreach (var rem in model.Allergies.Where(a => !dto.AllergyIds.Contains(a.Id)).ToList())
            model.Allergies.Remove(rem);

        // Add new allergies that are missing
        var existingAllergyIds = model.Allergies.Select(a => a.Id).ToHashSet();
        foreach (var allergy in targetAllergies.Where(a => !existingAllergyIds.Contains(a.Id)))
            model.Allergies.Add(allergy);

        var targetCategories = dbContext.StationCategories
            .Where(sc => dto.StationCategoryIds.Contains(sc.Id))
            .ToList();

        // Remove categories not present in the DTO
        foreach (var rem in model.StationCategories.Where(sc => !dto.StationCategoryIds.Contains(sc.Id)).ToList())
            model.StationCategories.Remove(rem);

        // Add new categories that are missing
        var existingCategoryIds = model.StationCategories.Select(sc => sc.Id).ToHashSet();
        foreach (var category in targetCategories.Where(sc => !existingCategoryIds.Contains(sc.Id)))
            model.StationCategories.Add(category);
    }
}