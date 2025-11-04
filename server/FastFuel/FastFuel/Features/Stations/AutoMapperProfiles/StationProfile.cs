using AutoMapper;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Models;

namespace FastFuel.Features.Stations.AutoMapperProfiles
{
    public class StationProfile : Profile
    {
        public StationProfile()
        {
            CreateMap<Station, StationDto>().ReverseMap();
            CreateMap<Station, EditStationDto>().ReverseMap();
        }
    }
}
