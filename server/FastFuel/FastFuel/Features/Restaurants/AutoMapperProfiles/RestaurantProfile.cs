using AutoMapper;
using FastFuel.Features.OpeningHours.Models;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.Restaurants.AutoMapperProfiles;

public class RestaurantProfile : Profile
{
    public RestaurantProfile()
    {
        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(dest => dest.OpeningHours, opt => opt.MapFrom(src => src.OpeningHours));
        
        CreateMap<EditRestaurantDto, Restaurant>();
        
        CreateMap<RestaurantOpeningHourDto, OpeningHour>()
            .ReverseMap();
    }
}