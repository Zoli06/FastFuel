using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.MenuFoods.Models;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Models;

namespace FastFuel.Features.Menus.Mappers;

public class MenuMapper : IMapper<Menu, MenuRequestDto, MenuResponseDto>
{
    public MenuResponseDto ToDto(Menu model)
    {
        return new MenuResponseDto
        {
            Id = model.Id,
            Name = model.Name,
            Price = model.Price,
            Description = model.Description,
            ImageUrl = model.ImageUrl,
            Foods = model.MenuFoods.ConvertAll(ToDto)
        };
    }

    public Menu ToModel(MenuRequestDto dto)
    {
        return new Menu
        {
            Name = dto.Name,
            Price = dto.Price,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            MenuFoods = dto.Foods.ConvertAll(ToModel)
        };
    }

    public void UpdateModel(MenuRequestDto dto, ref Menu model)
    {
        model.Name = dto.Name;
        model.Price = dto.Price;
        model.Description = dto.Description;
        model.ImageUrl = dto.ImageUrl;

        model.MenuFoods.Clear();
        model.MenuFoods.AddRange(dto.Foods
            .ConvertAll(ToModel));
    }

    private MenuFoodDto ToDto(MenuFood model)
    {
        return new MenuFoodDto
        {
            FoodId = model.FoodId,
            Quantity = model.Quantity
        };
    }

    private MenuFood ToModel(MenuFoodDto dto)
    {
        return new MenuFood
        {
            FoodId = dto.FoodId,
            Quantity = dto.Quantity
        };
    }
}