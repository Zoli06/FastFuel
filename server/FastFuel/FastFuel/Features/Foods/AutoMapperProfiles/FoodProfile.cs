using AutoMapper;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;

namespace FastFuel.Features.Foods.AutoMapperProfiles;

public class FoodProfile : Profile
{
    public FoodProfile()
    {
        CreateMap<Food, FoodDto>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.FoodIngredients));
        
        CreateMap<EditFoodDto, Food>()
            .ForMember(dest => dest.FoodIngredients, opt => opt.MapFrom(src => src.Ingredients));
    }
}