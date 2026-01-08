using FastFuel.Features.Common;
using FastFuel.Features.MenuFoods.Models;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Models;

namespace FastFuel.Features.Menus.Mappers;

public class MenuMapper : Mapper<Menu, MenuRequestDto, MenuResponseDto>
{
    public override MenuResponseDto ToDto(Menu model)
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

    private MenuFoodDto ToDto(MenuFood model)
    {
        return new MenuFoodDto
        {
            FoodId = model.FoodId,
            Quantity = model.Quantity
        };
    }

    public override Menu ToModel(MenuRequestDto dto)
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

    private MenuFood ToModel(MenuFoodDto dto)
    {
        return new MenuFood
        {
            FoodId = dto.FoodId,
            Quantity = dto.Quantity
        };
    }

    public override void UpdateModel(MenuRequestDto dto, ref Menu model)
    {
        model.Name = dto.Name;
        model.Price = dto.Price;
        model.Description = dto.Description;
        model.ImageUrl = dto.ImageUrl;

        model.MenuFoods.Clear();
        model.MenuFoods.AddRange(dto.Foods
            .ConvertAll(ToModel));
    }
}