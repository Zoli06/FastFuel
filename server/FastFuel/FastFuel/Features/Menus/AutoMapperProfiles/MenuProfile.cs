using AutoMapper;
using FastFuel.Features.MenuFoods.Models;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Models;

namespace FastFuel.Features.Menus.AutoMapperProfiles;

public class MenuProfile : Profile
{
    public MenuProfile()
    {
        CreateMap<Menu, MenuDto>()
            .ForMember(dest => dest.Foods, opt => opt.MapFrom(src => src.MenuFoods));

        CreateMap<EditMenuDto, Menu>();

        CreateMap<MenuFoodDto, MenuFood>()
            .ReverseMap();
    }
}