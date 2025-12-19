using AutoMapper;
using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Models;

namespace FastFuel.Features.Ingredients.AutoMapperProfiles;

public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        CreateMap<Ingredient, IngredientDto>()
            .ForMember(dest => dest.FoodIds, opt => opt.MapFrom(src => src.FoodIngredients.Select(fi => fi.FoodId).ToList()))
            .ForMember(dest => dest.AllergyIds, opt => opt.MapFrom(src => src.Allergies.Select(a => a.Id).ToList()))
            .ForMember(dest => dest.StationCategoryIds,
                opt => opt.MapFrom(src => src.StationCategories.Select(sc => sc.Id).ToList()));

        CreateMap<EditIngredientDto, Ingredient>()
            .ForMember(dest => dest.Allergies, opt => opt.Ignore())
            .ForMember(dest => dest.StationCategories, opt => opt.Ignore());
    }
}