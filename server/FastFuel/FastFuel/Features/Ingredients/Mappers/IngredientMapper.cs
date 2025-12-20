
using FastFuel.Features.Common;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Models;

namespace FastFuel.Features.Ingredients.Mappers;

public static class IngredientMapper
{
    public static IngredientResponseDto ToDto(this Ingredient ingredient)
    {
        return new IngredientResponseDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            ImageUrl = ingredient.ImageUrl,
            AllergyIds = ingredient.Allergies.ConvertAll(allergy => allergy.Id),
            StationCategoryIds = ingredient.StationCategories.ConvertAll(category => category.Id),
            FoodIds = ingredient.FoodIngredients.ConvertAll(fi => fi.FoodId)
        };
    }
    
    public static Ingredient ToModel(this IngredientRequestDto dto, ApplicationDbContext dbContext)
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
    
    public static void UpdateModel(this Ingredient ingredient, IngredientRequestDto dto, ApplicationDbContext dbContext)
    {
        ingredient.Name = dto.Name;
        ingredient.ImageUrl = dto.ImageUrl;

        var targetAllergies = dbContext.Allergies
            .Where(a => dto.AllergyIds.Contains(a.Id))
            .ToList();

        // Remove allergies not present in the DTO
        foreach (var rem in ingredient.Allergies.Where(a => !dto.AllergyIds.Contains(a.Id)).ToList())
            ingredient.Allergies.Remove(rem);

        // Add new allergies that are missing
        var existingAllergyIds = ingredient.Allergies.Select(a => a.Id).ToHashSet();
        foreach (var allergy in targetAllergies.Where(a => !existingAllergyIds.Contains(a.Id)))
            ingredient.Allergies.Add(allergy);

        var targetCategories = dbContext.StationCategories
            .Where(sc => dto.StationCategoryIds.Contains(sc.Id))
            .ToList();

        // Remove categories not present in the DTO
        foreach (var rem in ingredient.StationCategories.Where(sc => !dto.StationCategoryIds.Contains(sc.Id)).ToList())
            ingredient.StationCategories.Remove(rem);

        // Add new categories that are missing
        var existingCategoryIds = ingredient.StationCategories.Select(sc => sc.Id).ToHashSet();
        foreach (var category in targetCategories.Where(sc => !existingCategoryIds.Contains(sc.Id)))
            ingredient.StationCategories.Add(category);
    }
}