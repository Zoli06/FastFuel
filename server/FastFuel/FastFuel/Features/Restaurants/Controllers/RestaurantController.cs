using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Restaurants.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RestaurantController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRestaurants()
    {
        var restaurants = await context.Restaurants.ToListAsync();
        return Ok(mapper.Map<List<RestaurantDto>>(restaurants));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id)
    {
        var restaurant = await context.Restaurants.FindAsync(id);
        if (restaurant == null)
            return NotFound();

        return Ok(mapper.Map<RestaurantDto>(restaurant));
    }

    [HttpPost]
    public async Task<ActionResult<RestaurantDto>> CreateRestaurant(EditRestaurantDto restaurantDto)
    {
        var restaurant = mapper.Map<Restaurant>(restaurantDto);
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync();

        var createdRestaurantDto = mapper.Map<RestaurantDto>(restaurant);
        return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id }, createdRestaurantDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRestaurant(int id, EditRestaurantDto restaurantDto)
    {
        var restaurant = await context.Restaurants.FindAsync(id);
        if (restaurant == null)
            return NotFound();

        mapper.Map(restaurantDto, restaurant);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRestaurant(int id)
    {
        var restaurant = await context.Restaurants.FindAsync(id);
        if (restaurant == null)
            return NotFound();

        context.Restaurants.Remove(restaurant);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
