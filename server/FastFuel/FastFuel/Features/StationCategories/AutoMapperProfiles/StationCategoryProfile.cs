using AutoMapper;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.StationCategories.AutoMapperProfiles;

public class StationCategoryProfile : Profile
{
    public StationCategoryProfile()
    {
        CreateMap<StationCategory, StationCategoryDto>()
            .ForMember(dest => dest.IngredientIds, opt => opt.MapFrom(src => src.Ingredients.ConvertAll(i => i.Id)))
            .ForMember(dest => dest.StationIds, opt => opt.MapFrom(src => src.Stations.ConvertAll(s => s.Id)));

        CreateMap<EditStationCategoryDto, StationCategory>();
    }
}