using FastFuel.Features.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Mappers;
using FastFuel.Features.Orders.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Orders.Controllers;

// TODO: Allow staff to update order status and mark as completed
public class OrderController(ApplicationDbContext dbContext)
    : ApplicationController<Order, OrderRequestDto, OrderResponseDto>(dbContext)
{
    protected override Mapper<Order, OrderRequestDto, OrderResponseDto> Mapper =>
        new OrderMapper();

    protected override DbSet<Order> DbSet => DbContext.Orders;

    public override async Task<IActionResult> Create(OrderRequestDto requestDto)
    {
        var model = Mapper.ToModel(requestDto);

        // Reset order number to zero if it's 99 or above and order number 1 wasn't used for at least 3 hours
        var lastOrder = await DbSet
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
        if (lastOrder is { OrderNumber: >= 99 } &&
            lastOrder.CreatedAt.AddHours(3) < DateTime.UtcNow)
            model.OrderNumber = 1;
        else
            model.OrderNumber = (lastOrder?.OrderNumber ?? 0) + 1;

        await DbSet.AddAsync(model);
        await DbContext.SaveChangesAsync();
        var responseDto = Mapper.ToDto(model);
        return CreatedAtAction(nameof(GetById), new { id = model.Id }, responseDto);
    }

    public override async Task<IActionResult> Update(uint id, OrderRequestDto requestDto)
    {
        var model = await DbSet
            .FirstOrDefaultAsync(a => a.Id == id);

        if (model == null)
            return NotFound();

        if (model.Status != OrderStatus.Pending)
            return BadRequest("Only pending orders can be updated.");

        Mapper.UpdateModel(requestDto, ref model);
        await DbContext.SaveChangesAsync();
        return NoContent();
    }

    public override async Task<IActionResult> Delete(uint id)
    {
        var model = await DbSet
            .FirstOrDefaultAsync(a => a.Id == id);

        if (model == null)
            return NotFound();

        if (model.Status != OrderStatus.Pending)
            return BadRequest("Only pending orders can be deleted.");

        DbSet.Remove(model);
        await DbContext.SaveChangesAsync();
        return NoContent();
    }
}