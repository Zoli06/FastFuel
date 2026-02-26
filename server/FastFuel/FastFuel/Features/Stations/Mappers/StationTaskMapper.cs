using FastFuel.Features.FoodIngredients.Entities;
using FastFuel.Features.MenuFoods.Entities;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.StationCategories.Entities;
using FastFuel.Features.Stations.DTOs;

namespace FastFuel.Features.Stations.Mappers;

public class StationTaskMapper : IStationTasksMapper
{
    public StationTasksResponseDto ToDto(List<Order> orders, StationCategory stationCategory,
        Dictionary<uint, List<MenuFood>> relevantMenuFoodsByMenuId)
    {
        return new StationTasksResponseDto
        {
            Orders = orders.ConvertAll(o => ToDto(o, stationCategory, relevantMenuFoodsByMenuId))
        };
    }

    private static StationTaskOrder ToDto(Order order, StationCategory stationCategory,
        Dictionary<uint, List<MenuFood>> relevantMenuFoodsByMenuId)
    {
        return new StationTaskOrder
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CreatedAt = order.CreatedAt,
            Foods = order.Foods.ConvertAll(f => ToDto(f, stationCategory)),
            Menus = order.Menus.ConvertAll(m => ToDto(m, stationCategory,
                relevantMenuFoodsByMenuId.GetValueOrDefault(m.MenuId, []))),
            Status = order.Status
        };
    }

    private static StationTaskFood ToDto(OrderFood orderFood, StationCategory stationCategory)
    {
        return new StationTaskFood
        {
            Id = orderFood.Food.Id,
            Name = orderFood.Food.Name,
            Quantity = orderFood.Quantity,
            SpecialInstructions = orderFood.SpecialInstructions,
            Ingredients = orderFood.Food.FoodIngredients.ConvertAll(fi => ToDto(fi, stationCategory))
        };
    }

    private static StationTaskMenu ToDto(OrderMenu orderMenu, StationCategory stationCategory,
        List<MenuFood> relevantMenuFoods)
    {
        return new StationTaskMenu
        {
            Id = orderMenu.Menu.Id,
            Name = orderMenu.Menu.Name,
            Quantity = orderMenu.Quantity,
            SpecialInstructions = orderMenu.SpecialInstructions,
            Foods = relevantMenuFoods.ConvertAll(mf => ToDto(mf, stationCategory))
        };
    }

    private static StationTaskFood ToDto(MenuFood menuFood, StationCategory stationCategory)
    {
        return new StationTaskFood
        {
            Id = menuFood.Food.Id,
            Name = menuFood.Food.Name,
            Quantity = menuFood.Quantity,
            SpecialInstructions = null,
            Ingredients = menuFood.Food.FoodIngredients.ConvertAll(fi => ToDto(fi, stationCategory))
        };
    }

    private static StationTaskIngredient ToDto(FoodIngredient foodIngredient, StationCategory stationCategory)
    {
        return new StationTaskIngredient
        {
            Id = foodIngredient.Ingredient.Id,
            Name = foodIngredient.Ingredient.Name,
            Quantity = foodIngredient.Quantity,
            Unit = foodIngredient.Unit,
            IsRelevant = foodIngredient.Ingredient.StationCategories.Any(sc => sc.Id == stationCategory.Id)
        };
    }
}