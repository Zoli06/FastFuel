using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Mappers;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Restaurants.Controllers;

public class RestaurantController(ApplicationDbContext dbContext)
    : ApplicationController<Restaurant, RestaurantRequestDto, RestaurantResponseDto>(dbContext)
{
    private readonly PasswordHasher<Restaurant> _hasher = new();

    protected override Mapper<Restaurant, RestaurantRequestDto, RestaurantResponseDto> Mapper
        => new RestaurantMapper();

    protected override DbSet<Restaurant> DbSet => DbContext.Restaurants;

    public override async Task<Results<Created<RestaurantResponseDto>, Conflict<ProblemDetails>, UnauthorizedHttpResult>> Create(RestaurantRequestDto dto)
    {
        var restaurant = Mapper.ToModel(dto);

        restaurant.PasswordHash = _hasher.HashPassword(restaurant, dto.Password);
        
        DbSet.Add(restaurant);
        await DbContext.SaveChangesAsync();

        var response = Mapper.ToDto(restaurant);

        var location = Url.Action(nameof(GetById), new { id = response.Id });
        return TypedResults.Created(location!, response);
    }
}