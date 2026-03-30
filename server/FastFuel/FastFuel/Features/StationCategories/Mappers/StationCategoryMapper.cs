using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Entities;

namespace FastFuel.Features.StationCategories.Mappers;

public class StationCategoryMapper(ApplicationDbContext dbContext)
    : IMapper<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto>
{
    public StationCategoryResponseDto ToDto(StationCategory entity)
    {
        return new StationCategoryResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IngredientIds = entity.Ingredients.ConvertAll(ingredient => ingredient.Id),
            StationIds = entity.Stations.ConvertAll(station => station.Id)
        };
    }

    public StationCategory ToEntity(StationCategoryRequestDto dto)
    {
        return new StationCategory
        {
            Name = dto.Name,
            Ingredients = dbContext.Ingredients
                .Where(ingredient => dto.IngredientIds.Contains(ingredient.Id))
                .ToList()
        };
    }

    public void UpdateEntity(StationCategoryRequestDto dto, StationCategory entity)
    {
        entity.Name = dto.Name;

        entity.Ingredients.Clear();
        entity.Ingredients.AddRange(dbContext.Ingredients
            .Where(ingredient => dto.IngredientIds.Contains(ingredient.Id))
            .ToList());
    }
}