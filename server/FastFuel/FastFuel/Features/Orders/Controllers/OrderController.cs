using FastFuel.Features.Common.Controllers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Entities;

namespace FastFuel.Features.Orders.Controllers;

// TODO: Allow staff to update order status and mark as completed
public class OrderController(ICrudService<OrderRequestDto, OrderResponseDto> service)
    : CrudController<Order, OrderRequestDto, OrderResponseDto>(service);