using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Mappers;
using FastFuel.Features.Restaurants.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Restaurants.Controllers;

public class RestaurantController(ApplicationDbContext dbContext)
    : ApplicationController<Restaurant, RestaurantRequestDto, RestaurantResponseDto>(dbContext)
{
    protected override Mapper<Restaurant, RestaurantRequestDto, RestaurantResponseDto> Mapper => new RestaurantMapper();
    protected override DbSet<Restaurant> DbSet => DbContext.Restaurants;
}