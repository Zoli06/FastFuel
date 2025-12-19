using AutoMapper;
using FastFuel.Features.OrderFoods.Models;
using FastFuel.Features.OrderMenus.Models;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Models;

namespace FastFuel.Features.Orders.AutoMapperProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<PlaceOrderDto, Order>();
        
        CreateMap<OrderFood, OrderFoodDto>()
            .ReverseMap();
        CreateMap<OrderMenu, OrderMenuDto>()
            .ReverseMap();
    }
}