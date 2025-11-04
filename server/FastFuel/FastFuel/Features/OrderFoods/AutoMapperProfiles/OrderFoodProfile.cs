using AutoMapper;
using FastFuel.Features.OrderFoods.DTOs;
using FastFuel.Features.OrderFoods.Models;

namespace FastFuel.Features.OrderFoods.AutoMapperProfiles
{
    public class OrderFoodProfile : Profile
    {
        public OrderFoodProfile()
        {
            CreateMap<OrderFood, OrderFoodDto>().ReverseMap();
            CreateMap<OrderFood, EditOrderFoodDto>().ReverseMap();
        }
    }
}
