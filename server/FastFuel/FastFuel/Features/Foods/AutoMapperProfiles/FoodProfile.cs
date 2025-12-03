using System.Linq;
using AutoMapper;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;
using FastFuel.Features.FoodIngredients.Models;

namespace FastFuel.Features.Foods.AutoMapperProfiles;

public class FoodProfile : Profile
{
    public FoodProfile()
    {
        CreateMap<Food, FoodDto>()
            .ForMember(dest => dest.IngredientIds,
                opt => opt.MapFrom(src => src.FoodIngredients.Select(fi => fi.IngredientId).ToList()))
            .ReverseMap()
            .ForMember(dest => dest.FoodIngredients,
                opt => opt.MapFrom(src => src.IngredientIds.Select(id => new FoodIngredient { IngredientId = id }).ToList()));

        CreateMap<Food, EditFoodDto>()
            .ForMember(dest => dest.IngredientIds,
                opt => opt.MapFrom(src => src.FoodIngredients.Select(fi => fi.IngredientId).ToList()))
            .ReverseMap()
            .ForMember(dest => dest.FoodIngredients,
                opt => opt.MapFrom(src => src.IngredientIds.Select(id => new FoodIngredient { IngredientId = id }).ToList()));
    }
}