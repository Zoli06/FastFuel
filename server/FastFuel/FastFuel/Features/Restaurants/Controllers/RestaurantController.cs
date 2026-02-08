using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Models;

namespace FastFuel.Features.Restaurants.Controllers;

public class RestaurantController(ICrudService<RestaurantRequestDto, RestaurantResponseDto> service)
    : CrudController<Restaurant, RestaurantRequestDto, RestaurantResponseDto>(service);