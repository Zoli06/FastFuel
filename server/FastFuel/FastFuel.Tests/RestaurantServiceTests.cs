using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.Restaurants.Mappers;
using FastFuel.Features.Restaurants.Services;

namespace FastFuel.Tests;

public class RestaurantServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private RestaurantService _service = null!;

    // ─── Lifecycle ─────────────────────────────────────────────
    public Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        var mapper = new RestaurantMapper();
        _service = new RestaurantService(_dbContext, mapper);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _dbContext.Restaurants.RemoveRange(_dbContext.Restaurants);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────
    private static List<RestaurantOpeningHourDto> DefaultOpeningHours()
    {
        return new List<RestaurantOpeningHourDto>
        {
            new() { DayOfWeek = DayOfWeek.Monday, OpenTime = new TimeOnly(9,0), CloseTime = new TimeOnly(17,0) },
            new() { DayOfWeek = DayOfWeek.Tuesday, OpenTime = new TimeOnly(9,0), CloseTime = new TimeOnly(17,0) }
        };
    }

    private static RestaurantRequestDto BuildRequest(
        string name = "Test Restaurant",
        string? description = "Test description",
        string address = "123 Main St",
        double latitude = 40.7128,
        double longitude = -74.0060,
        string? phone = null,
        List<RestaurantOpeningHourDto>? openingHours = null)
    {
        return new RestaurantRequestDto
        {
            Name = name,
            Description = description,
            Address = address,
            Latitude = latitude,
            Longitude = longitude,
            Phone = phone,
            OpeningHours = openingHours ?? DefaultOpeningHours()
        };
    }

    private async Task<RestaurantResponseDto> CreateRestaurantAsync(
        string name = "Test Restaurant")
    {
        return await _service.CreateAsync(BuildRequest(name));
    }

    private async Task<int> GetAllCountAsync()
    {
        var all = await _service.GetAllAsync();
        return all.Count;
    }

    // ─── GetAll ─────────────────────────────────────────────────
    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenRestaurantsExist_ReturnsAll()
    {
        await CreateRestaurantAsync("Restaurant A");
        await CreateRestaurantAsync("Restaurant B");
        await CreateRestaurantAsync("Restaurant C");

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, r => r.Name == "Restaurant A");
        Assert.Contains(result, r => r.Name == "Restaurant B");
        Assert.Contains(result, r => r.Name == "Restaurant C");
    }

    // ─── GetById ───────────────────────────────────────────────
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsRestaurant()
    {
        var created = await CreateRestaurantAsync("My Restaurant");

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("My Restaurant", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);
        Assert.Null(result);
    }

    // ─── Create ────────────────────────────────────────────────
    [Fact]
    public async Task CreateAsync_PersistsRestaurant()
    {
        var request = BuildRequest(
            name: "Burger Place",
            description: "Best burgers in town",
            address: "456 Broadway",
            latitude: 41.0,
            longitude: -73.5,
            phone: "555-1234"
        );

        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal("Burger Place", result.Name);
        Assert.Equal("Best burgers in town", result.Description);
        Assert.Equal("456 Broadway", result.Address);
        Assert.Equal(41.0, result.Latitude);
        Assert.Equal(-73.5, result.Longitude);
        Assert.Equal("555-1234", result.Phone);
        Assert.Equal(2, result.OpeningHours.Count); // from DefaultOpeningHours
    }

    [Fact]
    public async Task CreateAsync_WithNullFields_PersistsNulls()
    {
        var request = BuildRequest(description: null, phone: null);
        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Null(result.Description);
        Assert.Null(result.Phone);
        Assert.Equal(2, result.OpeningHours.Count); // DefaultOpeningHours
    }

    [Fact]
    public async Task CreateAsync_IncreasesCount()
    {
        await CreateRestaurantAsync("A");
        await CreateRestaurantAsync("B");

        var count = await GetAllCountAsync();

        Assert.Equal(2, count);
    }

    // ─── Update ────────────────────────────────────────────────
    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesRestaurant()
    {
        var created = await CreateRestaurantAsync("Old Name");

        var updateRequest = BuildRequest(
            name: "New Name",
            description: "Updated description",
            address: "789 Park Ave",
            latitude: 42.0,
            longitude: -72.0,
            phone: "555-9999"
        );

        var success = await _service.UpdateAsync(created.Id, updateRequest);

        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal("Updated description", updated.Description);
        Assert.Equal("789 Park Ave", updated.Address);
        Assert.Equal(42.0, updated.Latitude);
        Assert.Equal(-72.0, updated.Longitude);
        Assert.Equal("555-9999", updated.Phone);
        Assert.Equal(2, updated.OpeningHours.Count); // preserved DefaultOpeningHours
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());
        Assert.False(success);
    }

    // ─── Delete ────────────────────────────────────────────────
    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesRestaurant()
    {
        var created = await CreateRestaurantAsync("Restaurant X");

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
    public async Task DeleteAsync_DecreasesCount()
    {
        var a = await CreateRestaurantAsync("A");
        var b = await CreateRestaurantAsync("B");

        await _service.DeleteAsync(a.Id);

        var all = await _service.GetAllAsync();

        Assert.Single(all);
        Assert.Equal(b.Id, all[0].Id);
    }
}