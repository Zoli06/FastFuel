using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Orders.Services;

public class OrderService(ApplicationDbContext dbContext, IMapper<Order, OrderRequestDto, OrderResponseDto> mapper)
    : CrudService<Order, OrderRequestDto, OrderResponseDto>(dbContext, mapper)
{
    private const int MinOrderNumberBeforeReset = 99;
    private const int MinHoursBeforeReset = 3;
    protected override DbSet<Order> DbSet => DbContext.Orders;

    protected override Create<Order, OrderRequestDto, OrderResponseDto> CreateOperation =>
        new Create(DbContext, DbSet, Mapper);

    protected override Update<Order, OrderRequestDto, OrderResponseDto> UpdateOperation =>
        new Update(DbContext, DbSet, Mapper);

    protected override Delete<Order> DeleteOperation => new Delete(DbContext, DbSet);

    private static uint GetNextOrderNumber(Order? lastOrder)
    {
        if (lastOrder is { OrderNumber: >= MinOrderNumberBeforeReset } &&
            lastOrder.CreatedAt.AddHours(MinHoursBeforeReset) < DateTime.UtcNow)
            return 1;
        return (lastOrder?.OrderNumber ?? 0) + 1;
    }

    private static void EnsurePendingStatus(Order model)
    {
        if (model.Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be modified.");
    }

    private class Create(
        ApplicationDbContext dbContext,
        DbSet<Order> dbSet,
        IMapper<Order, OrderRequestDto, OrderResponseDto> mapper)
        : Create<Order, OrderRequestDto, OrderResponseDto>(dbContext, dbSet, mapper)
    {
        protected override async Task<Order> CreateModelAsync(OrderRequestDto requestDto)
        {
            var model = await base.CreateModelAsync(requestDto);

            var lastOrder = await DbSet
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();
            model.OrderNumber = GetNextOrderNumber(lastOrder);

            return model;
        }
    }

    private class Update(
        ApplicationDbContext dbContext,
        DbSet<Order> dbSet,
        IMapper<Order, OrderRequestDto, OrderResponseDto> mapper)
        : Update<Order, OrderRequestDto, OrderResponseDto>(dbContext, dbSet, mapper)
    {
        protected override Task SaveModelAsync(uint id, OrderRequestDto requestDto, Order model)
        {
            EnsurePendingStatus(model);
            return base.SaveModelAsync(id, requestDto, model);
        }
    }

    private class Delete(ApplicationDbContext dbContext, DbSet<Order> dbSet) : Delete<Order>(dbContext, dbSet)
    {
        protected override Task DeleteModelAsync(uint id, Order model)
        {
            EnsurePendingStatus(model);
            return base.DeleteModelAsync(id, model);
        }
    }
}