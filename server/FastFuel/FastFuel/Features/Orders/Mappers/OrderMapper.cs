using FastFuel.Features.Common;
using FastFuel.Features.OrderFoods.Models;
using FastFuel.Features.OrderMenus.Models;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Models;

namespace FastFuel.Features.Orders.Mappers;

public class OrderMapper : Mapper<Order, OrderRequestDto, OrderResponseDto>
{
    public override OrderResponseDto ToDto(Order model)
    {
        return new OrderResponseDto
        {
            Id = model.Id,
            RestaurantId = model.RestaurantId,
            OrderNumber = model.OrderNumber,
            Status = model.Status.ToString(),
            CreatedAt = model.CreatedAt,
            CompletedAt = model.CompletedAt,
            Menus = model.Menus.ConvertAll(ToDto),
            Foods = model.Foods.ConvertAll(ToDto)
        };
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

    public override Order ToModel(OrderRequestDto dto)
    {
        return new Order
        {
            RestaurantId = dto.RestaurantId,
            Menus = dto.Menus.ConvertAll(ToModel),
            Foods = dto.Foods.ConvertAll(ToModel)
        };
    }

    private OrderFood ToModel(OrderFoodDto dto)
    {
        return new OrderFood
        {
            FoodId = dto.FoodId,
            Quantity = dto.Quantity,
            SpecialInstructions = dto.SpecialInstructions
        };
    }

    private OrderMenu ToModel(OrderMenuDto dto)
    {
        return new OrderMenu
        {
            MenuId = dto.MenuId,
            Quantity = dto.Quantity,
            SpecialInstructions = dto.SpecialInstructions
        };
    }

    public override void UpdateModel(OrderRequestDto dto, ref Order model)
    {
        model.RestaurantId = dto.RestaurantId;

        model.Menus.Clear();
        model.Menus.AddRange(dto.Menus.ConvertAll(ToModel));

        model.Foods.Clear();
        model.Foods.AddRange(dto.Foods.ConvertAll(ToModel));
    }
}