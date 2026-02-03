using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Models;
using FastFuel.Features.Orders.Services;

namespace FastFuel.Features.Orders.Controllers;

// TODO: Allow staff to update order status and mark as completed
public class OrderController(ApplicationDbContext dbContext)
    : CrudController<Order, OrderRequestDto, OrderResponseDto>
{
    protected override CrudService<Order, OrderRequestDto, OrderResponseDto> Service { get; } = new OrderService(dbContext);
}