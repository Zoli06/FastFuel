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

        model.Allergies.Clear();
        model.Allergies.AddRange(dbContext.Allergies
            .Where(a => dto.AllergyIds.Contains(a.Id))
            .ToList());

        model.StationCategories.Clear();
        model.StationCategories.AddRange(dbContext.StationCategories
            .Where(sc => dto.StationCategoryIds.Contains(sc.Id))
            .ToList());
    }
}