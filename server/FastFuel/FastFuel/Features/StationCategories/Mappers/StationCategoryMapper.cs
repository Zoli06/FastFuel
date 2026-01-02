using FastFuel.Features.Common;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.StationCategories.Mappers;

public class StationCategoryMapper(ApplicationDbContext dbContext) : Mapper<StationCategory, StationCategoryRequestDto, StationCategoryResponseDto>
{
    public override StationCategoryResponseDto ToDto(StationCategory model)
    {
        return new StationCategoryResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            IngredientIds = model.Ingredients.ConvertAll(ingredient => ingredient.Id),
            StationIds = model.Stations.ConvertAll(station => station.Id)
        };
    }

    public override StationCategory ToModel(StationCategoryRequestDto dto)
    {
        return new StationCategory
        {
            Name = dto.Name,
            Ingredients = dbContext.Ingredients
                .Where(ingredient => dto.IngredientIds.Contains(ingredient.Id))
                .ToList(),
            Stations = dbContext.Stations
                .Where(station => dto.StationIds.Contains(station.Id))
                .ToList()
        };
    }

    public override void UpdateModel(StationCategoryRequestDto dto, ref StationCategory model)
    {
        model.Name = dto.Name;
        
        model.Ingredients.Clear();
        model.Ingredients.AddRange(dbContext.Ingredients
            .Where(ingredient => dto.IngredientIds.Contains(ingredient.Id))
            .ToList());
        
        model.Stations.Clear();
        model.Stations.AddRange(dbContext.Stations
            .Where(station => dto.StationIds.Contains(station.Id))
            .ToList());
    }
}