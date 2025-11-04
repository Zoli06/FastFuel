using AutoMapper;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.StationCategories.AutoMapperProfiles
{
    public class StationCategoryProfile : Profile
    {
        public StationCategoryProfile()
        {
            CreateMap<StationCategory, StationCategoryDto>().ReverseMap();
            CreateMap<StationCategory, EditStationCategoryDto>().ReverseMap();
        }
    }
}
