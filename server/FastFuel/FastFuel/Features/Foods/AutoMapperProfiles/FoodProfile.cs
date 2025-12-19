using AutoMapper;
using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;

namespace FastFuel.Features.Foods.AutoMapperProfiles;

public class FoodProfile : Profile
{
    public FoodProfile()
    {
        CreateMap<Food, FoodDto>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.FoodIngredients))
            .ForMember(dest => dest.MenuIds, opt => opt.MapFrom(src => src.MenuFoods.Select(mf => mf.MenuId).ToList()));

        CreateMap<EditFoodDto, Food>()
            .ForMember(dest => dest.FoodIngredients, opt => opt.MapFrom(src => src.Ingredients));

        CreateMap<FoodIngredient, FoodIngredientDto>()
            .ReverseMap();
    }
}