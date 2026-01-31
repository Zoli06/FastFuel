using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Models;
using FastFuel.Features.Restaurants.Services;

namespace FastFuel.Features.Restaurants.Controllers;

public class RestaurantController(ApplicationDbContext dbContext)
    : CrudController<Restaurant, RestaurantRequestDto, RestaurantResponseDto>
{
    protected override CrudService<Restaurant, RestaurantRequestDto, RestaurantResponseDto> Service { get; } = new RestaurantService(dbContext);
}