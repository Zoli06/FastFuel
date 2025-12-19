using AutoMapper;
using FastFuel.Features.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Orders.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class OrderController(ApplicationDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var orders = await context.Orders.ToListAsync();
        return Ok(mapper.Map<List<OrderDto>>(orders));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetOrder(uint id)
    {
        var order = await context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        return Ok(mapper.Map<OrderDto>(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(PlaceOrderDto orderDto)
    {
        var order = mapper.Map<Order>(orderDto);
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var createdOrderDto = mapper.Map<OrderDto>(order);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, createdOrderDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(uint id)
    {
        var order = await context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        context.Orders.Remove(order);
        await context.SaveChangesAsync();

        return NoContent();
    }
}