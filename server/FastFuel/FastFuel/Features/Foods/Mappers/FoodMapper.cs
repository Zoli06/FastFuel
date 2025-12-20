using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;

namespace FastFuel.Features.Foods.Mappers;

public static class FoodMapper
{
    public static FoodResponseDto ToDto(this Food food)
    {
        return new FoodResponseDto
        {
            Id = food.Id,
            Name = food.Name,
            Price = food.Price,
            Description = food.Description,
            ImageUrl = food.ImageUrl,
            Ingredients = food.FoodIngredients
                .Select(fi => fi.ToDto())
                .ToList(),
            MenuIds = food.MenuFoods
                .Select(mf => mf.MenuId)
                .ToList()
        };
    }
    
    private static FoodIngredientDto ToDto(this FoodIngredient foodIngredient)
    {
        return new FoodIngredientDto
        {
            IngredientId = foodIngredient.IngredientId,
            Quantity = foodIngredient.Quantity
        };
    }
    
    public static Food ToModel(this FoodRequestDto dto)
    {
        return new Food
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            FoodIngredients = dto.Ingredients
                .Select(i => i.ToModel())
                .ToList()
        };
    }
    
    private static FoodIngredient ToModel(this FoodIngredientDto dto)
    {
        return new FoodIngredient
        {
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity
        };
    }
    
    public static void UpdateModel(this Food food, FoodRequestDto dto)
    {
        food.Name = dto.Name;
        food.Price = dto.Price;
        food.Description = dto.Description;
        food.ImageUrl = dto.ImageUrl;

        // Update FoodIngredients
        // Remove ingredients not present in the DTO
        foreach (var rem in food.FoodIngredients
                     .Where(fi => dto.Ingredients.All(i => i.IngredientId != fi.IngredientId))
                     .ToList())
        {
            food.FoodIngredients.Remove(rem);
        }

        // Add or update ingredients from the DTO
        foreach (var ingredientDto in dto.Ingredients)
        {
            var existingFi = food.FoodIngredients
                .FirstOrDefault(fi => fi.IngredientId == ingredientDto.IngredientId);
            if (existingFi != null)
            {
                // Update quantity if it exists
                existingFi.Quantity = ingredientDto.Quantity;
            }
            else
            {
                // Add new FoodIngredient
                food.FoodIngredients.Add(ingredientDto.ToModel());
            }
        }
    }
}