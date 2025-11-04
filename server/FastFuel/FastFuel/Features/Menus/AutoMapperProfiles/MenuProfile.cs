using AutoMapper;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Models;
namespace FastFuel.Features.Menus.AutoMapperProfiles
{
    public class MenuProfile : Profile
    {

        public MenuProfile()
        {
            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Menu, EditMenuDto>().ReverseMap();
        }
    }
}
