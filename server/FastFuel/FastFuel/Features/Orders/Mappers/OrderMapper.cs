using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Entities;

namespace FastFuel.Features.Orders.Mappers;

public class OrderMapper : IMapper<Order, OrderRequestDto, OrderResponseDto>
{
    public OrderResponseDto ToDto(Order entity)
    {
        return new OrderResponseDto
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            RestaurantId = entity.RestaurantId,
            OrderNumber = entity.OrderNumber,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            CompletedAt = entity.CompletedAt,
            Menus = entity.Menus.ConvertAll(ToDto),
            Foods = entity.Foods.ConvertAll(ToDto)
        };
    }

    public Order ToEntity(OrderRequestDto dto)
    {
        return new Order
        {
            //CustomerId = dto.CustomerId,
            RestaurantId = dto.RestaurantId,
            Menus = dto.Menus.ConvertAll(ToEntity),
            Foods = dto.Foods.ConvertAll(ToEntity)
        };
    }

    public void UpdateEntity(OrderRequestDto dto, Order entity)
    {
        // if (dto.CustomerId != entity.CustomerId)
        //     throw new InvalidOperationException("Cannot change the CustomerId of an order.");

        entity.RestaurantId = dto.RestaurantId;

        entity.Menus.Clear();
        entity.Menus.AddRange(dto.Menus.ConvertAll(ToEntity));

        entity.Foods.Clear();
        entity.Foods.AddRange(dto.Foods.ConvertAll(ToEntity));
    }

    private OrderFoodDto ToDto(OrderFood orderFood)
    {
        return new OrderFoodDto
        {
            FoodId = orderFood.FoodId,
            Quantity = orderFood.Quantity,
            SpecialInstructions = orderFood.SpecialInstructions
        };
    }

    private OrderMenuDto ToDto(OrderMenu orderMenu)
    {
        return new OrderMenuDto
        {
            MenuId = orderMenu.MenuId,
            Quantity = orderMenu.Quantity,
            SpecialInstructions = orderMenu.SpecialInstructions
        };
    }

    private OrderFood ToEntity(OrderFoodDto dto)
    {
        return new OrderFood
        {
            FoodId = dto.FoodId,
            Quantity = dto.Quantity,
            SpecialInstructions = dto.SpecialInstructions
        };
    }

    private OrderMenu ToEntity(OrderMenuDto dto)
    {
        return new OrderMenu
        {
            MenuId = dto.MenuId,
            Quantity = dto.Quantity,
            SpecialInstructions = dto.SpecialInstructions
        };
    }
}