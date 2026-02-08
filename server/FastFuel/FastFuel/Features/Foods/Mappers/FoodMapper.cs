using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;

namespace FastFuel.Features.Foods.Mappers;

public class FoodMapper : IMapper<Food, FoodRequestDto, FoodResponseDto>
{
    public FoodResponseDto ToDto(Food model)
    {
        return new FoodResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Price = model.Price,
            Description = model.Description,
            ImageUrl = model.ImageUrl,
            Ingredients = model.FoodIngredients
                .ConvertAll(ToDto),
            MenuIds = model.MenuFoods
                .ConvertAll(mf => mf.MenuId)
        };
    }

    public Food ToModel(FoodRequestDto dto)
    {
        return new Food
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            FoodIngredients = dto.Ingredients
                .ConvertAll(ToModel)
        };
    }

    public void UpdateModel(FoodRequestDto dto, ref Food model)
    {
        model.Name = dto.Name;
        model.Price = dto.Price;
        model.Description = dto.Description;
        model.ImageUrl = dto.ImageUrl;

        model.FoodIngredients.Clear();
        model.FoodIngredients.AddRange(dto.Ingredients
            .ConvertAll(ToModel));
    }

    private FoodIngredientDto ToDto(FoodIngredient model)
    {
        return new FoodIngredientDto
        {
            IngredientId = model.IngredientId,
            Quantity = model.Quantity
        };
    }

    private FoodIngredient ToModel(FoodIngredientDto dto)
    {
        return new FoodIngredient
        {
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity
        };
    }
}