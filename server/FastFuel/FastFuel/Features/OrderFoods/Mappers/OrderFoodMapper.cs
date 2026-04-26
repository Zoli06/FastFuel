using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.OrderFoods.DTOs;
using FastFuel.Features.OrderFoods.Entities;

namespace FastFuel.Features.OrderFoods.Mappers;

public class OrderFoodMapper : IMapper<OrderFood, OrderFoodRequestDto, OrderFoodResponseDto>
{
    public OrderFoodResponseDto ToDto(OrderFood entity)
    {
        return new OrderFoodResponseDto
        {
            FoodId = entity.FoodId,
            OriginalFoodName = entity.OriginalFoodName,
            OriginalFoodPrice = entity.OriginalFoodPrice,
            Quantity = entity.Quantity,
            SpecialInstructions = entity.SpecialInstructions
        };
    }

    public OrderFood ToEntity(OrderFoodRequestDto dto)
    {
        return new OrderFood
        {
            FoodId = dto.FoodId,
            Quantity = dto.Quantity,
            SpecialInstructions = dto.SpecialInstructions
        };
    }

    public void UpdateEntity(OrderFoodRequestDto dto, OrderFood entity)
    {
        entity.FoodId = dto.FoodId;
        entity.Quantity = dto.Quantity;
        entity.SpecialInstructions = dto.SpecialInstructions;
    }
}