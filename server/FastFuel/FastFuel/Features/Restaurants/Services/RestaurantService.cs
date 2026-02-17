using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Restaurants.Services;

public class RestaurantService(
    ApplicationDbContext dbContext,
    IMapper<Restaurant, RestaurantRequestDto, RestaurantResponseDto> mapper,
    IPasswordHasher<Restaurant> passwordHasher)
    : CrudService<Restaurant, RestaurantRequestDto, RestaurantResponseDto>(dbContext, mapper)
{
    protected override DbSet<Restaurant> DbSet { get; } = dbContext.Restaurants;

    protected override Task OnBeforeCreateModelAsync(Restaurant model, RestaurantRequestDto requestDto)
    {
        if (requestDto.Password == null)
            throw new ArgumentException("Password is required for restaurant creation.");
        model.PasswordHash = passwordHasher.HashPassword(model, requestDto.Password);
        return Task.CompletedTask;
    }

    protected override Task OnBeforeUpdateModelAsync(Restaurant model, RestaurantRequestDto requestDto)
    {
        if (requestDto.Password != null) model.PasswordHash = passwordHasher.HashPassword(model, requestDto.Password);
        return Task.CompletedTask;
    }
}