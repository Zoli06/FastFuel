using FastFuel.Features.Common;
using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;

namespace FastFuel.Features.Foods.Mappers;

public class FoodMapper : Mapper<Food, FoodRequestDto, FoodResponseDto>
{
    public override FoodResponseDto ToDto(Food model)
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

    private FoodIngredientDto ToDto(FoodIngredient model)
    {
        return new FoodIngredientDto
        {
            IngredientId = model.IngredientId,
            Quantity = model.Quantity
        };
    }

    public override Food ToModel(FoodRequestDto dto)
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

    private FoodIngredient ToModel(FoodIngredientDto dto)
    {
        return new FoodIngredient
        {
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity
        };
    }

    public override void UpdateModel(FoodRequestDto dto, ref Food model)
    {
        model.Name = dto.Name;
        model.Price = dto.Price;
        model.Description = dto.Description;
        model.ImageUrl = dto.ImageUrl;

        // Update FoodIngredients
        // Remove ingredients not present in the DTO
        foreach (var rem in model.FoodIngredients
                     .Where(fi => dto.Ingredients.All(i => i.IngredientId != fi.IngredientId))
                     .ToList())
            model.FoodIngredients.Remove(rem);

        // Add or update ingredients from the DTO
        foreach (var ingredientDto in dto.Ingredients)
        {
            var existingFi = model.FoodIngredients
                .FirstOrDefault(fi => fi.IngredientId == ingredientDto.IngredientId);
            if (existingFi != null)
                // Update quantity if it exists
                existingFi.Quantity = ingredientDto.Quantity;
            else
                // Add new FoodIngredient
                model.FoodIngredients.Add(ToModel(ingredientDto));
        }
    }
}