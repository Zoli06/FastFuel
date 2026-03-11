using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Foods.Entities;
using FastFuel.Features.Menus.Entities;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Mappers;
using FastFuel.Features.Orders.Services;
using FastFuel.Features.Orders.Services.OrderFilter;
using FastFuel.Features.Restaurants.Entities;

namespace FastFuel.Tests;

public class OrderServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private Food _food = null!;
    private Menu _menu = null!;

    private Restaurant _restaurant = null!;
    private IOrderService _service = null!;

    // ─── Lifecycle ─────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        _service = new OrderService(_dbContext, new OrderMapper());

        // Seed required data dynamically
        _restaurant = new Restaurant { Name = "Test Restaurant" };
        _food = new Food { Name = "Test Food", Price = 1000 };
        _menu = new Menu { Name = "Test Menu" };

        _dbContext.Restaurants.Add(_restaurant);
        _dbContext.Foods.Add(_food);
        _dbContext.Menus.Add(_menu);

        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up all data to avoid unique constraint violations
        _dbContext.Orders.RemoveRange(_dbContext.Orders);
        _dbContext.Restaurants.RemoveRange(_dbContext.Restaurants);
        _dbContext.Foods.RemoveRange(_dbContext.Foods);
        _dbContext.Menus.RemoveRange(_dbContext.Menus);

        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────

    private OrderRequestDto BuildRequest(
        uint? restaurantId = null,
        List<uint>? foodIds = null,
        List<uint>? menuIds = null)
    {
        var foods = (foodIds ?? new List<uint> { _food.Id })
            .Select(id => new OrderFoodDto
            {
                FoodId = id,
                Quantity = 1,
                SpecialInstructions = null
            })
            .ToList();

        var menus = (menuIds ?? new List<uint> { _menu.Id })
            .Select(id => new OrderMenuDto
            {
                MenuId = id,
                Quantity = 1,
                SpecialInstructions = null
            })
            .ToList();

        return new OrderRequestDto
        {
            RestaurantId = restaurantId ?? _restaurant.Id,
            Foods = foods,
            Menus = menus
        };
    }

    private ClaimsPrincipal BuildUser(uint customerId)
    {
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customerId.ToString())
            },
            "TestAuth"
        );

        return new ClaimsPrincipal(identity);
    }

    private async Task CreateOrdersAsync(int count)
    {
        for (var i = 0; i < count; i++) await _service.CreateAsync(BuildRequest());
    }

    // ─── Tests ────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenOrdersExist_ReturnsAll()
    {
        await CreateOrdersAsync(3);
        var result = await _service.GetAllAsync();
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsOrder()
    {
        var created = await _service.CreateAsync(BuildRequest());
        var result = await _service.GetByIdAsync(created.Id);
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_PersistsOrder()
    {
        var request = BuildRequest();
        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal(_restaurant.Id, result.RestaurantId);
        Assert.Single(result.Foods);
        Assert.Single(result.Menus);
    }

    [Fact]
    public async Task CreateAsync_IncreasesCount()
    {
        await CreateOrdersAsync(2);
        var all = await _service.GetAllAsync();
        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesOrder()
    {
        var created = await _service.CreateAsync(BuildRequest());
        var success = await _service.UpdateAsync(created.Id, BuildRequest());
        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Equal(_restaurant.Id, updated!.RestaurantId);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());
        Assert.False(success);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesOrder()
    {
        var created = await _service.CreateAsync(BuildRequest());
        var success = await _service.DeleteAsync(created.Id);
        Assert.True(success);

        var deleted = await _service.GetByIdAsync(created.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.DeleteAsync(99999);
        Assert.False(success);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithValidId_UpdatesStatus()
    {
        var created = await _service.CreateAsync(BuildRequest());
        var success = await _service.UpdateOrderStatusAsync(created.Id, OrderStatus.Completed);
        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Equal(OrderStatus.Completed, updated!.Status);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateOrderStatusAsync(99999, OrderStatus.Completed);
        Assert.False(success);
    }

    [Fact]
    public async Task GetOrdersForCurrentUserAsync_ReturnsUserOrders()
    {
        var userId = 10u;
        await CreateOrdersAsync(2);

        var user = BuildUser(userId);
        var result = await _service.GetOrdersForCurrentUserAsync(user);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAllOrdersWithFiltersAsync_ReturnsFilteredOrders()
    {
        await CreateOrdersAsync(2);

        var filter = new OrderFilterParams { Status = OrderStatus.Pending };
        var result = await _service.GetAllOrdersWithFiltersAsync(filter);

        Assert.NotNull(result);
    }
}