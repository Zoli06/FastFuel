using AutoMapper;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Models;

namespace FastFuel.Features.StationCategories.AutoMapperProfiles
{
    public class StationCategorieProfile : Profile
    {
        public StationCategorieProfile()
        {
            CreateMap<StationCategory, StationCategorieDto>().ReverseMap();
            CreateMap<StationCategory, EditStationCategorieDto>().ReverseMap();
        }
    }
}
