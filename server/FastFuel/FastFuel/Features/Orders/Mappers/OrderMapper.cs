using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OrderFoods.DTOs;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.DTOs;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Entities;

namespace FastFuel.Features.Orders.Mappers;

public class OrderMapper(
    IMapper<OrderFood, OrderFoodRequestDto, OrderFoodResponseDto> orderFoodMapper,
    IMapper<OrderMenu, OrderMenuRequestDto, OrderMenuResponseDto> orderMenuMapper)
    : IMapper<Order, OrderRequestDto, OrderResponseDto>
{
    public OrderResponseDto ToDto(Order entity)
    {
        return new OrderResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            RestaurantId = entity.RestaurantId,
            OrderNumber = entity.OrderNumber,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            CompletedAt = entity.CompletedAt,
            Menus = entity.Menus.ConvertAll(orderMenuMapper.ToDto),
            Foods = entity.Foods.ConvertAll(orderFoodMapper.ToDto)
        };
    }

    public Order ToEntity(OrderRequestDto dto)
    {
        return new Order
        {
            RestaurantId = dto.RestaurantId,
            Menus = dto.Menus.ConvertAll(orderMenuMapper.ToEntity),
            Foods = dto.Foods.ConvertAll(orderFoodMapper.ToEntity)
        };
    }

    public void UpdateEntity(OrderRequestDto dto, Order entity)
    {
        entity.RestaurantId = dto.RestaurantId;

        entity.Menus.Clear();
        entity.Menus.AddRange(dto.Menus.ConvertAll(orderMenuMapper.ToEntity));

        entity.Foods.Clear();
        entity.Foods.AddRange(dto.Foods.ConvertAll(orderFoodMapper.ToEntity));
    }
}