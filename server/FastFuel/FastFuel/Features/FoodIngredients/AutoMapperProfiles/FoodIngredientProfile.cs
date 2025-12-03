using AutoMapper;
using FastFuel.Features.FoodIngredients.DTOs;
using FastFuel.Features.FoodIngredients.Models;

namespace FastFuel.Features.FoodIngredients.AutoMapperProfiles;

public class FoodIngredientProfile : Profile
{
    public FoodIngredientProfile()
    {
        CreateMap<FoodIngredient, FoodIngredientDto>()
            .ReverseMap();
    }
}