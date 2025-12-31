using AutoMapper;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Models;

namespace FastFuel.Features.Stations.AutoMapperProfiles;

public class StationProfile : Profile
{
    public StationProfile()
    {
        CreateMap<Station, StationDto>()
            .ForMember(dest => dest.StationCategoryId, opt => opt.MapFrom(src => src.StationCategoryId))
            .ForMember(dest => dest.RestaurantId, opt => opt.MapFrom(src => src.RestaurantId));

        CreateMap<EditStationDto, Station>();
    }
}