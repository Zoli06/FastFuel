using AutoMapper;
using FastFuel.Features.OrderMenus.DTOs;
using FastFuel.Features.OrderMenus.Models;

namespace FastFuel.Features.OrderMenus.AutoMapperProfiles
{
    public class OrderMenuProfile : Profile
    {
        public OrderMenuProfile()
        {
            CreateMap<OrderMenu, OrderMenuDto>().ReverseMap();
            CreateMap<OrderMenu, EditOrderMenuDto>().ReverseMap();
        }
    }
}

