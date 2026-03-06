using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Entities;

namespace FastFuel.Features.Ingredients.Mappers;

public class IngredientMapper(ApplicationDbContext dbContext)
    : IMapper<Ingredient, IngredientRequestDto, IngredientResponseDto>
{
    public IngredientResponseDto ToDto(Ingredient entity)
    {
        return new IngredientResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageUrl = entity.ImageUrl,
            AllergyIds = entity.Allergies.ConvertAll(allergy => allergy.Id),
            StationCategoryIds = entity.StationCategories.ConvertAll(category => category.Id),
            FoodIds = entity.FoodIngredients.ConvertAll(fi => fi.FoodId),
            DefaultTimerValueSeconds = (uint)entity.DefaultTimerValue.TotalSeconds
        };
    }

    public Ingredient ToEntity(IngredientRequestDto dto)
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
                .ToList(),
            DefaultTimerValue = TimeSpan.FromSeconds(dto.DefaultTimerValueSeconds)
        };
    }

    public void UpdateEntity(IngredientRequestDto dto, Ingredient entity)
    {
        entity.Name = dto.Name;
        entity.ImageUrl = dto.ImageUrl;

        entity.Allergies.Clear();
        entity.Allergies.AddRange(dbContext.Allergies
            .Where(a => dto.AllergyIds.Contains(a.Id))
            .ToList());

        entity.StationCategories.Clear();
        entity.StationCategories.AddRange(dbContext.StationCategories
            .Where(sc => dto.StationCategoryIds.Contains(sc.Id))
            .ToList());
        entity.DefaultTimerValue = TimeSpan.FromSeconds(dto.DefaultTimerValueSeconds);
    }
}