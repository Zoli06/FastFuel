using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.FoodIngredients.Entities;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Entities;

namespace FastFuel.Features.Foods.Mappers;

public class FoodMapper : IMapper<Food, FoodRequestDto, FoodResponseDto>
{
    public FoodResponseDto ToDto(Food entity)
    {
        return new FoodResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            Description = entity.Description,
            ImageUrl = entity.ImageUrl,
            Ingredients = entity.FoodIngredients
                .ConvertAll(ToDto),
            MenuIds = entity.MenuFoods
                .ConvertAll(mf => mf.MenuId)
        };
    }

    public Food ToEntity(FoodRequestDto dto)
    {
        return new Food
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            FoodIngredients = dto.Ingredients
                .ConvertAll(ToEntity)
        };
    }

    public void UpdateEntity(FoodRequestDto dto, Food entity)
    {
        entity.Name = dto.Name;
        entity.Price = dto.Price;
        entity.Description = dto.Description;
        entity.ImageUrl = dto.ImageUrl;

        entity.FoodIngredients.Clear();
        entity.FoodIngredients.AddRange(dto.Ingredients
            .ConvertAll(ToEntity));
    }

    private FoodIngredientDto ToDto(FoodIngredient entity)
    {
        return new FoodIngredientDto
        {
            IngredientId = entity.IngredientId,
            Quantity = entity.Quantity
        };
    }

    private FoodIngredient ToEntity(FoodIngredientDto dto)
    {
        return new FoodIngredient
        {
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity
        };
    }
}