using FastFuel.Features.MenuFoods.Entities;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.StationCategories.Entities;
using FastFuel.Features.Stations.DTOs;

namespace FastFuel.Features.Stations.Mappers;

public interface IStationTasksMapper
{
    StationTasksResponseDto ToDto(List<Order> orders, StationCategory stationCategory,
        Dictionary<uint, List<MenuFood>> relevantMenuFoodsByMenuId);
}