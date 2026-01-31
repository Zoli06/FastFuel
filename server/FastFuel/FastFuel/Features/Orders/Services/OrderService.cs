using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Mappers;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Mappers;
using FastFuel.Features.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Orders.Services;

public class OrderService(ApplicationDbContext dbContext)
    : CrudService<Order, OrderRequestDto, OrderResponseDto>(dbContext)
{
    private const int MinOrderNumberBeforeReset = 99;
    private const int MinHoursBeforeReset = 3;
    protected override Mapper<Order, OrderRequestDto, OrderResponseDto> Mapper { get; } = new OrderMapper();
    protected override DbSet<Order> DbSet => DbContext.Orders;

    private static uint GetNextOrderNumber(Order? lastOrder)
    {
        if (lastOrder is { OrderNumber: >= MinOrderNumberBeforeReset } &&
            lastOrder.CreatedAt.AddHours(MinHoursBeforeReset) < DateTime.UtcNow)
            return 1;
        return (lastOrder?.OrderNumber ?? 0) + 1;
    }

    protected override Task OnBeforeCreateModelAsync(Order model)
    {
        return Task.Run(async () =>
        {
            var lastOrder = await DbSet
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
            model.OrderNumber = GetNextOrderNumber(lastOrder);
        });
    }

    private static void EnsurePendingStatus(Order model)
    {
        if (model.Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be modified.");
    }

    protected override Task OnBeforeUpdateModelAsync(Order model)
    {
        return Task.Run(() => { EnsurePendingStatus(model); });
    }

    protected override Task OnBeforeDeleteModelAsync(Order model)
    {
        return Task.Run(() => { EnsurePendingStatus(model); });
    }
}