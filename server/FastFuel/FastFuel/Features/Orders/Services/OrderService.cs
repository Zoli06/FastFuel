using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Common.Services.CrudOperations;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Entities;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Entities;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Orders.Services;

public class OrderService(
    FastFuelDbContext dbContext,
    IMapper<Order, OrderRequestDto, OrderResponseDto> mapper,
    ICrudService<FoodRequestDto, FoodResponseDto> foodService,
    ICrudService<MenuRequestDto, MenuResponseDto> menuService)
    : IOrderService
{
    private const int MinOrderNumberBeforeReset = 99;
    private const int MinHoursBeforeReset = 3;
    protected DbSet<Order> DbSet => dbContext.Orders;


    protected virtual GetAll<Order, OrderRequestDto, OrderResponseDto> GetAllOperation => new(DbSet, mapper);
    protected virtual GetById<Order, OrderRequestDto, OrderResponseDto> GetByIdOperation => new(DbSet, mapper);

    protected Create<Order, OrderRequestDto, OrderResponseDto> CreateOperation =>
        new Create(dbContext, DbSet, mapper, foodService, menuService);

    protected virtual Delete<Order> DeleteOperation => new(dbContext, DbSet);

    public Task<List<OrderResponseDto>> GetAllAsync(uint? userId = null, CancellationToken cancellationToken = default)
    {
        return GetAllOperation.ExecuteAsync(userId, cancellationToken);
    }

    public Task<OrderResponseDto?> GetByIdAsync(uint id, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return GetByIdOperation.ExecuteAsync(id, userId, cancellationToken);
    }

    public Task<OrderResponseDto> CreateAsync(OrderRequestDto requestDto, uint? userId = null,
        CancellationToken cancellationToken = default)
    {
        return CreateOperation.ExecuteAsync(requestDto, userId, cancellationToken);
    }

    public Task<bool> DeleteAsync(uint id, uint? userId = null, CancellationToken cancellationToken = default)
    {
        return DeleteOperation.ExecuteAsync(id, userId, cancellationToken);
    }

    public async Task<List<OrderResponseDto>> GetOrdersForCurrentUserAsync(ClaimsPrincipal user,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new ResourceNotFoundAppException(nameof(ClaimsPrincipal), nameof(user));

        if (!uint.TryParse(userIdClaim.Value, out var userId))
            throw new ResourceNotFoundAppException(nameof(ClaimsPrincipal), nameof(userIdClaim));

        var orders = await DbSet
            .Include(o => o.Foods)
            .Include(o => o.Menus)
            .Where(o => o.UserId == userId)
            .ToListAsync(cancellationToken);

        return orders.ConvertAll(mapper.ToDto);
    }

    public async Task<List<OrderResponseDto>> GetAllOrdersWithFiltersAsync(OrderFilterParams filterParams,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(o => o.Foods)
            .Include(o => o.Menus)
            .AsQueryable();

        if (filterParams.Status.HasValue)
            query = query.Where(o => o.Status == filterParams.Status.Value);

        if (filterParams.RestaurantId.HasValue)
            query = query.Where(o => o.RestaurantId == filterParams.RestaurantId.Value);

        var orders = await query.ToListAsync(cancellationToken);
        return orders.ConvertAll(mapper.ToDto);
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

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static uint GetNextOrderNumber(Order? lastOrder)
    {
        if (lastOrder is { OrderNumber: >= MinOrderNumberBeforeReset } &&
            lastOrder.CreatedAt.AddHours(MinHoursBeforeReset) < DateTime.UtcNow)
            return 1;
        return (lastOrder?.OrderNumber ?? 0) + 1;
    }

    private class Create(
        FastFuelDbContext dbContext,
        DbSet<Order> dbSet,
        IMapper<Order, OrderRequestDto, OrderResponseDto> mapper,
        ICrudService<FoodRequestDto, FoodResponseDto> foodService,
        ICrudService<MenuRequestDto, MenuResponseDto> menuService)
        : Create<Order, OrderRequestDto, OrderResponseDto>(dbContext, dbSet, mapper)
    {
        private static readonly SemaphoreSlim OrderCreationLock = new(1, 1);

        public override async Task<OrderResponseDto> ExecuteAsync(OrderRequestDto requestDto, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            await OrderCreationLock.WaitAsync(cancellationToken);
            try
            {
                await using var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken);

                var entity = await CreateEntityAsync(requestDto, userId, cancellationToken);
                await SaveEntityAsync(requestDto, entity, userId, cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return await CreateDtoAsync(requestDto, entity, userId, cancellationToken);
            }
            finally
            {
                OrderCreationLock.Release();
            }
        }

        protected override async Task<Order> CreateEntityAsync(OrderRequestDto requestDto, uint? userId = null,
            CancellationToken cancellationToken = default)
        {
            var entity = await base.CreateEntityAsync(requestDto, userId, cancellationToken);

            var lastOrder = await DbSet
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
            entity.OrderNumber = GetNextOrderNumber(lastOrder);

            if (userId == null)
                throw new AppException("User ID is required to create an order.");
            entity.UserId = userId.Value;

            foreach (var food in entity.Foods)
            {
                var originalFood = foodService.GetByIdAsync(food.FoodId, null, cancellationToken).Result
                                   ?? throw new ResourceNotFoundAppException(nameof(Food), food.FoodId);
                food.OriginalFoodName = originalFood.Name;
                food.OriginalFoodPrice = originalFood.Price;
            }

            foreach (var menu in entity.Menus)
            {
                var originalMenu = menuService.GetByIdAsync(menu.MenuId, null, cancellationToken).Result
                                   ?? throw new ResourceNotFoundAppException(nameof(Menu), menu.MenuId);
                menu.OriginalMenuName = originalMenu.Name;
                menu.OriginalMenuPrice = originalMenu.Price;
            }

            return entity;
        }
    }
}