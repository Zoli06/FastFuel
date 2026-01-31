using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Mappers;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Restaurants.Services;

public class RestaurantService(ApplicationDbContext dbContext) : CrudService<Restaurant, RestaurantRequestDto, RestaurantResponseDto>(dbContext)
{
    protected override Mapper<Restaurant, RestaurantRequestDto, RestaurantResponseDto> Mapper { get; } =
        new RestaurantMapper();
    protected override DbSet<Restaurant> DbSet => DbContext.Restaurants;

    protected override Task OnBeforeCreateModelAsync(Restaurant model)
    {
        var hasher = new PasswordHasher<Restaurant>();
        model.PasswordHash = hasher.HashPassword(model, model.PasswordHash);
        return Task.CompletedTask;
    }
}