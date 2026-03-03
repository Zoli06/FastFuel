using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.Orders.Services.OrderFilter;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Orders.Services;

public class OrderService(
    ApplicationDbContext dbContext,
    IMapper<Order, OrderRequestDto, OrderResponseDto> mapper)
    : CrudService<Order, OrderRequestDto, OrderResponseDto>(dbContext, mapper), IOrderService
{
    private const int MinOrderNumberBeforeReset = 99;
    private const int MinHoursBeforeReset = 3;
    protected override DbSet<Order> DbSet => DbContext.Orders;

    protected override Create<Order, OrderRequestDto, OrderResponseDto> CreateOperation =>
        new Create(DbContext, DbSet, Mapper);

    protected override Update<Order, OrderRequestDto, OrderResponseDto> UpdateOperation =>
        new Update(DbContext, DbSet, Mapper);

    protected override Delete<Order> DeleteOperation => new Delete(DbContext, DbSet);

    public async Task<List<OrderResponseDto>> GetOrdersForCurrentUserAsync(ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new InvalidOperationException("User ID claim not found.");

        if (!uint.TryParse(userIdClaim.Value, out var userId))
            throw new InvalidOperationException("Invalid user ID claim value.");

        var orders = await DbSet
            .Include(o => o.Foods)
            .Include(o => o.Menus)
            .Where(o => o.CustomerId == userId)
            .ToListAsync(cancellationToken);

        return orders.ConvertAll(Mapper.ToDto);
    }

    public async Task<List<OrderResponseDto>> GetAllOrdersWithFiltersAsync(IOrderFilterParams filterParams,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(o => o.Foods)
            .Include(o => o.Menus)
            .AsQueryable();

        if (filterParams.Status.HasValue)
            query = query.Where(o => o.Status == filterParams.Status.Value);

        var orders = await query.ToListAsync(cancellationToken);
        return orders.ConvertAll(Mapper.ToDto);
    }

    public async Task<bool> UpdateOrderStatusAsync(uint orderId, OrderStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        var order = await DbSet.FindAsync([orderId], cancellationToken);
        if (order == null)
            return false;

        order.Status = newStatus;
        if (newStatus == OrderStatus.Completed)
            order.CompletedAt = DateTime.UtcNow;

        await DbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static uint GetNextOrderNumber(Order? lastOrder)
    {
        if (lastOrder is { OrderNumber: >= MinOrderNumberBeforeReset } &&
            lastOrder.CreatedAt.AddHours(MinHoursBeforeReset) < DateTime.UtcNow)
            return 1;
        return (lastOrder?.OrderNumber ?? 0) + 1;
    }

    private static void EnsurePendingStatus(Order entity)
    {
        if (entity.Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be modified.");
    }

    private static async Task<uint> CalculatePriceAsync(Order entity, ApplicationDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var menuIds = entity.Menus.Select(m => m.MenuId).ToList();
        var foodIds = entity.Foods.Select(f => f.FoodId).ToList();

        var menuPrices = await dbContext.Menus
            .Where(m => menuIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id, m => m.Price, cancellationToken);

        var foodPrices = await dbContext.Foods
            .Where(f => foodIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id, f => f.Price, cancellationToken);

        var menuPrice = (uint)entity.Menus.Sum(m => m.Quantity * menuPrices.GetValueOrDefault(m.MenuId));
        var foodPrice = (uint)entity.Foods.Sum(f => f.Quantity * foodPrices.GetValueOrDefault(f.FoodId));
        return menuPrice + foodPrice;
    }

    private class Create(
        ApplicationDbContext dbContext,
        DbSet<Order> dbSet,
        IMapper<Order, OrderRequestDto, OrderResponseDto> mapper)
        : Create<Order, OrderRequestDto, OrderResponseDto>(dbContext, dbSet, mapper)
    {
        protected override async Task<Order> CreateEntityAsync(OrderRequestDto requestDto, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await base.CreateEntityAsync(requestDto, userId, cancellationToken);

            var lastOrder = await DbSet
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
            entity.OrderNumber = GetNextOrderNumber(lastOrder);

            if (await DbContext.Customers.AnyAsync(c => c.Id == userId, cancellationToken))
                entity.CustomerId = userId;

            entity.Price = await CalculatePriceAsync(entity, DbContext, cancellationToken);

            return entity;
        }
    }

    private class Update(
        ApplicationDbContext dbContext,
        DbSet<Order> dbSet,
        IMapper<Order, OrderRequestDto, OrderResponseDto> mapper)
        : Update<Order, OrderRequestDto, OrderResponseDto>(dbContext, dbSet, mapper)
    {
        protected override async Task UpdateEntityAsync(uint id, OrderRequestDto requestDto, Order entity,
            uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            await base.UpdateEntityAsync(id, requestDto, entity, userId, cancellationToken);
            entity.Price = await CalculatePriceAsync(entity, DbContext, cancellationToken);
        }

        protected override Task SaveEntityAsync(uint id, OrderRequestDto requestDto, Order entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            EnsurePendingStatus(entity);
            return base.SaveEntityAsync(id, requestDto, entity, userId, cancellationToken);
        }
    }

    private class Delete(ApplicationDbContext dbContext, DbSet<Order> dbSet) : Delete<Order>(dbContext, dbSet)
    {
        protected override Task DeleteEntityAsync(uint id, Order entity, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            EnsurePendingStatus(entity);
            return base.DeleteEntityAsync(id, entity, userId, cancellationToken);
        }
    }
}