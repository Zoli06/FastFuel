using AutoMapper;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.Restaurants.AutoMapperProfiles
{
    public class RestaurantProfile : Profile
    {
        public RestaurantProfile()
        {
            CreateMap<Restaurant, RestaurantDto>().ReverseMap();
            CreateMap<Restaurant, EditRestaurantDto>().ReverseMap();
        }
    }
}

