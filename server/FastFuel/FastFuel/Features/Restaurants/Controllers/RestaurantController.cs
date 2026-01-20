using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Mappers;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Restaurants.Controllers;

public class RestaurantController(ApplicationDbContext dbContext)
    : ApplicationController<Restaurant, RestaurantRequestDto, RestaurantResponseDto>(dbContext)
{
    private readonly PasswordHasher<Restaurant> _hasher = new PasswordHasher<Restaurant>();

    protected override Mapper<Restaurant, RestaurantRequestDto, RestaurantResponseDto> Mapper
        => new RestaurantMapper();

    protected override DbSet<Restaurant> DbSet => DbContext.Restaurants;

    public override async Task<IActionResult> Create([FromBody] RestaurantRequestDto dto)
    {
        if (DbSet.Any(r => r.Name == dto.Name))
            return Conflict(new { Message = "A restaurant with this name already exists." });

        var restaurant = Mapper.ToModel(dto);

        restaurant.PasswordHash = _hasher.HashPassword(restaurant, dto.Password);
        
        DbSet.Add(restaurant);
        await DbContext.SaveChangesAsync();

        var response = Mapper.ToDto(restaurant);

        return CreatedAtAction(nameof(Create), new { id = restaurant.Id }, response);
    }
}