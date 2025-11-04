using AutoMapper;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Models;

namespace FastFuel.Features.Orders.AutoMapperProfiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, EditOrderDto>().ReverseMap();
        }
    }
}