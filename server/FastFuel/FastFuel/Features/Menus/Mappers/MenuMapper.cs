using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.MenuFoods.Entities;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Entities;

namespace FastFuel.Features.Menus.Mappers;

public class MenuMapper : IMapper<Menu, MenuRequestDto, MenuResponseDto>
{
    public MenuResponseDto ToDto(Menu entity)
    {
        return new MenuResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            Description = entity.Description,
            ImageUrl = entity.ImageUrl,
            Foods = entity.MenuFoods.ConvertAll(ToDto)
        };
    }

    public Menu ToEntity(MenuRequestDto dto)
    {
        return new Menu
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            MenuFoods = dto.Foods.ConvertAll(ToEntity)
        };
    }

    public void UpdateEntity(MenuRequestDto dto, Menu entity)
    {
        entity.Name = dto.Name;
        entity.Price = dto.Price;
        entity.Description = dto.Description;
        entity.ImageUrl = dto.ImageUrl;

        entity.MenuFoods.Clear();
        entity.MenuFoods.AddRange(dto.Foods
            .ConvertAll(ToEntity));
    }

    private MenuFoodDto ToDto(MenuFood entity)
    {
        return new MenuFoodDto
        {
            FoodId = entity.FoodId,
            Quantity = entity.Quantity
        };
    }

    private MenuFood ToEntity(MenuFoodDto dto)
    {
        return new MenuFood
        {
            FoodId = dto.FoodId,
            Quantity = dto.Quantity
        };
    }
}