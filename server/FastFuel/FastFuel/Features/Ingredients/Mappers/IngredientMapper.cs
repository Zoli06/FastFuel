using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Entities;

namespace FastFuel.Features.Ingredients.Mappers;

public class IngredientMapper(FastFuelDbContext dbContext)
    : IMapper<Ingredient, IngredientRequestDto, IngredientResponseDto>
{
    public IngredientResponseDto ToDto(Ingredient entity)
    {
        return new IngredientResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            AllergyIds = entity.Allergies.ConvertAll(allergy => allergy.Id),
            StationCategoryIds = entity.StationCategories.ConvertAll(category => category.Id),
            FoodIds = entity.FoodIngredients.ConvertAll(fi => fi.FoodId)
        };
    }

    public Ingredient ToEntity(IngredientRequestDto dto)
    {
        return new Ingredient
        {
            Name = dto.Name,
            Allergies = dbContext.Allergies
                .Where(a => dto.AllergyIds.Contains(a.Id))
                .ToList(),
            StationCategories = dbContext.StationCategories
                .Where(sc => dto.StationCategoryIds.Contains(sc.Id))
                .ToList()
        };
    }

    public void UpdateEntity(IngredientRequestDto dto, Ingredient entity)
    {
        entity.Name = dto.Name;

        entity.Allergies.Clear();
        entity.Allergies.AddRange(dbContext.Allergies
            .Where(a => dto.AllergyIds.Contains(a.Id))
            .ToList());

        entity.StationCategories.Clear();
        entity.StationCategories.AddRange(dbContext.StationCategories
            .Where(sc => dto.StationCategoryIds.Contains(sc.Id))
            .ToList());
    }
}