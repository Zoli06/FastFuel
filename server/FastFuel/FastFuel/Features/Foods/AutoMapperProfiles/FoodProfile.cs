using AutoMapper;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Models;

namespace FastFuel.Features.Foods.AutoMapperProfiles;

public class FoodProfile : Profile
{
    public FoodProfile()
    {
        CreateMap<Food, FoodDto>().ReverseMap();
        CreateMap<Food, EditFoodDto>().ReverseMap();
    }
}