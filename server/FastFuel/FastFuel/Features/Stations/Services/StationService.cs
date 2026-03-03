using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.MenuFoods.Entities;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.StationCategories.Entities;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Entities;
using FastFuel.Features.Stations.Mappers;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Stations.Services;

public class StationService(
    ApplicationDbContext dbContext,
    IMapper<Station, StationRequestDto, StationResponseDto> mapper,
    IStationTasksMapper tasksMapper)
    : CrudService<Station, StationRequestDto, StationResponseDto>(dbContext, mapper), IStationService
{
    protected override DbSet<Station> DbSet { get; } = dbContext.Stations;

    public async Task<StationTasksResponseDto?> GetTasksForStationAsync(uint stationId,
        CancellationToken cancellationToken = default)
    {
        var station = await DbSet.FindAsync([stationId], cancellationToken);
        if (station == null)
            return null;

        var stationCategory = station.StationCategory;

        var relevantFoodIds = await GetRelevantFoodIdsAsync(stationCategory, cancellationToken);
        var relevantMenuFoods = await GetRelevantMenuFoodsAsync(relevantFoodIds, cancellationToken);
        var relevantMenuIds = relevantMenuFoods.Select(mf => mf.MenuId).Distinct().ToList();
        var relevantMenuFoodsByMenuId =
            relevantMenuFoods.GroupBy(mf => mf.MenuId).ToDictionary(g => g.Key, g => g.ToList());

        var orderFoods = await GetActiveOrderFoodsAsync(station.RestaurantId, relevantFoodIds, cancellationToken);
        var orderMenus = await GetActiveOrderMenusAsync(station.RestaurantId, relevantMenuIds, cancellationToken);

        var orders = BuildFilteredOrders(orderFoods, orderMenus);

        return tasksMapper.ToDto(orders, stationCategory, relevantMenuFoodsByMenuId);
    }

    private async Task<List<uint>> GetRelevantFoodIdsAsync(StationCategory stationCategory,
        CancellationToken cancellationToken)
    {
        return await DbContext.FoodIngredients
            .Where(fi => fi.Ingredient.StationCategories.Any(sc => sc.Id == stationCategory.Id))
            .Select(fi => fi.FoodId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<List<MenuFood>> GetRelevantMenuFoodsAsync(List<uint> relevantFoodIds,
        CancellationToken cancellationToken)
    {
        return await DbContext.MenuFoods
            .Where(mf => relevantFoodIds.Contains(mf.FoodId))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<OrderFood>> GetActiveOrderFoodsAsync(uint restaurantId, List<uint> relevantFoodIds,
        CancellationToken cancellationToken)
    {
        return await DbContext.OrderFoods
            .Where(f =>
                f.Order.RestaurantId == restaurantId &&
                (f.Order.Status == OrderStatus.Pending || f.Order.Status == OrderStatus.InProgress) &&
                relevantFoodIds.Contains(f.FoodId))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<OrderMenu>> GetActiveOrderMenusAsync(uint restaurantId, List<uint> relevantMenuIds,
        CancellationToken cancellationToken)
    {
        return await DbContext.OrderMenus
            .Where(m =>
                m.Order.RestaurantId == restaurantId &&
                (m.Order.Status == OrderStatus.Pending || m.Order.Status == OrderStatus.InProgress) &&
                relevantMenuIds.Contains(m.MenuId))
            .ToListAsync(cancellationToken);
    }

    private static List<Order> BuildFilteredOrders(List<OrderFood> orderFoods, List<OrderMenu> orderMenus)
    {
        var foodsByOrder = orderFoods.GroupBy(f => f.OrderId).ToDictionary(g => g.Key, g => g.ToList());
        var menusByOrder = orderMenus.GroupBy(m => m.OrderId).ToDictionary(g => g.Key, g => g.ToList());

        var orderIds = foodsByOrder.Keys.Union(menusByOrder.Keys);

        return orderIds.Select(orderId =>
        {
            var order = foodsByOrder.GetValueOrDefault(orderId)?.First().Order
                        ?? menusByOrder[orderId].First().Order;
            return new Order
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                RestaurantId = order.RestaurantId,
                CustomerId = order.CustomerId,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                CompletedAt = order.CompletedAt,
                Foods = foodsByOrder.GetValueOrDefault(orderId, []),
                Menus = menusByOrder.GetValueOrDefault(orderId, [])
            };
        }).ToList();
    }
}