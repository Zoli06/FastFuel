using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.OrderFoods.DTOs;
using FastFuel.Features.OrderFoods.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.OrderFoods.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class OrderFoodController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderFoodDto>>> GetOrderFoods()
    {
        var orderFoods = await context.OrderFoods.ToListAsync();
        return Ok(mapper.Map<List<OrderFoodDto>>(orderFoods));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderFoodDto>> GetOrderFood(int id)
    {
        var orderFood = await context.OrderFoods.FindAsync(id);
        if (orderFood == null)
            return NotFound();

        return Ok(mapper.Map<OrderFoodDto>(orderFood));
    }

    [HttpPost]
    public async Task<ActionResult<OrderFoodDto>> CreateOrderFood(EditOrderFoodDto orderFoodDto)
    {
        var orderFood = mapper.Map<OrderFood>(orderFoodDto);
        context.OrderFoods.Add(orderFood);
        await context.SaveChangesAsync();

        var createdOrderFoodDto = mapper.Map<OrderFoodDto>(orderFood);
        return CreatedAtAction(nameof(GetOrderFood), new { id = orderFood.Id }, createdOrderFoodDto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrderFood(int id, EditOrderFoodDto orderFoodDto)
    {
        var orderFood = await context.OrderFoods.FindAsync(id);
        if (orderFood == null)
            return NotFound();

        mapper.Map(orderFoodDto, orderFood);
        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrderFood(int id)
    {
        var orderFood = await context.OrderFoods.FindAsync(id);
        if (orderFood == null)
            return NotFound();

        context.OrderFoods.Remove(orderFood);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
