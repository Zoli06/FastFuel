using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Foods.Mappers;
using FastFuel.Features.Foods.Services;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Tests;

public class FoodServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private FoodService _service = null!;

    // ─── Lifecycle ──────────────────────────────────────────────────────────────

    public Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        var mapper = new FoodMapper();
        _service = new FoodService(_dbContext, mapper);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _dbContext.Foods.RemoveRange(_dbContext.Foods);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────

    private static FoodRequestDto BuildRequest(
        string name = "TestFood",
        uint price = 1000,
        string? description = "Test description",
        Uri? imageUrl = null)
    {
        return new FoodRequestDto
        {
            Name = name,
            Price = price,
            Description = description,
            ImageUrl = imageUrl,
            Ingredients = []
        };
    }

    // ─── GetAll ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFoods()
    {
        await _service.CreateAsync(BuildRequest("Pizza"));
        await _service.CreateAsync(BuildRequest("Burger"));
        await _service.CreateAsync(BuildRequest("Pasta"));

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, x => x.Name == "Pizza");
        Assert.Contains(result, x => x.Name == "Burger");
        Assert.Contains(result, x => x.Name == "Pasta");
    }

    // ─── GetById ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectFood()
    {
        var created = await _service.CreateAsync(BuildRequest("Steak", 5000));

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal("Steak", result.Name);
        Assert.Equal(5000u, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);

        Assert.Null(result);
    }

    // ─── Create ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_PersistsFood()
    {
        var request = BuildRequest("Taco", 2000);

        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal("Taco", result.Name);
        Assert.Equal(2000u, result.Price);

        var inDb = await _dbContext.Foods.FirstOrDefaultAsync(x => x.Id == result.Id);
        Assert.NotNull(inDb);
    }

    [Fact]
    public async Task CreateAsync_IncreasesCount()
    {
        await _service.CreateAsync(BuildRequest("A"));
        await _service.CreateAsync(BuildRequest("B"));

        var all = await _service.GetAllAsync();

        Assert.Equal(2, all.Count);
    }

    // ─── Update ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesFood()
    {
        var created = await _service.CreateAsync(BuildRequest("OldFood"));

        var updateRequest = BuildRequest("NewFood", 3000, "Updated description");

        var success = await _service.UpdateAsync(created.Id, updateRequest);

        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);

        Assert.Equal("NewFood", updated!.Name);
        Assert.Equal(3000u, updated.Price);
        Assert.Equal("Updated description", updated.Description);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());

        Assert.False(success);
    }

    // ─── Delete ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesFood()
    {
        var created = await _service.CreateAsync(BuildRequest("DeleteMe"));

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
        var f1 = await _service.CreateAsync(BuildRequest("A"));
        var f2 = await _service.CreateAsync(BuildRequest("B"));

        await _service.DeleteAsync(f1.Id);

        var all = await _service.GetAllAsync();

        Assert.Single(all);
        Assert.Equal(f2.Id, all[0].Id);
    }
}