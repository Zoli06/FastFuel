using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OrderMenus.DTOs;
using FastFuel.Features.OrderMenus.Entities;

namespace FastFuel.Features.OrderMenus.Mappers;

public class OrderMenuMapper : IMapper<OrderMenu, OrderMenuRequestDto, OrderMenuResponseDto>
{
    public OrderMenuResponseDto ToDto(OrderMenu entity)
    {
        return new OrderMenuResponseDto
        {
            MenuId = entity.MenuId,
            OriginalMenuName = entity.OriginalMenuName,
            OriginalMenuPrice = entity.OriginalMenuPrice,
            Quantity = entity.Quantity,
            SpecialInstructions = entity.SpecialInstructions
        };
    }

    public OrderMenu ToEntity(OrderMenuRequestDto dto)
    {
        return new OrderMenu
        {
            MenuId = dto.MenuId,
            Quantity = dto.Quantity,
            SpecialInstructions = dto.SpecialInstructions
        };
    }

    public void UpdateEntity(OrderMenuRequestDto dto, OrderMenu entity)
    {
        entity.MenuId = dto.MenuId;
        entity.Quantity = dto.Quantity;
        entity.SpecialInstructions = dto.SpecialInstructions;
    }
}