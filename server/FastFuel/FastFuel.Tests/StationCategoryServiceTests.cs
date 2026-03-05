using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Mappers;
using FastFuel.Features.StationCategories.Services;

namespace FastFuel.Tests;

public class StationCategoryServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private StationCategoryService _service = null!;

    // ─── Lifecycle ─────────────────────────────────────────────
    public Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        var mapper = new StationCategoryMapper(_dbContext);
        _service = new StationCategoryService(_dbContext, mapper);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _dbContext.StationCategories.RemoveRange(_dbContext.StationCategories);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────
    private static StationCategoryRequestDto BuildRequest(
        string name = "Test Category",
        List<uint>? ingredientIds = null,
        List<uint>? stationIds = null)
    {
        return new StationCategoryRequestDto
        {
            Name = name,
            IngredientIds = ingredientIds ?? new List<uint>(),
            StationIds = stationIds ?? new List<uint>()
        };
    }

    private async Task<StationCategoryResponseDto> CreateCategoryAsync(
        string name = "Test Category",
        List<uint>? ingredientIds = null,
        List<uint>? stationIds = null)
    {
        return await _service.CreateAsync(BuildRequest(name, ingredientIds, stationIds));
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
    public async Task GetAllAsync_WhenCategoriesExist_ReturnsAll()
    {
        await CreateCategoryAsync("Category A");
        await CreateCategoryAsync("Category B");
        await CreateCategoryAsync("Category C");

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, r => r.Name == "Category A");
        Assert.Contains(result, r => r.Name == "Category B");
        Assert.Contains(result, r => r.Name == "Category C");
    }

    // ─── GetById ───────────────────────────────────────────────
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCategory()
    {
        var created = await CreateCategoryAsync("My Category");

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("My Category", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);
        Assert.Null(result);
    }

    // ─── Create ────────────────────────────────────────────────
    [Fact]
    public async Task CreateAsync_PersistsCategory()
    {
        var ingredients = new List<uint> { 1, 2 };
        var stations = new List<uint> { 10, 20 };

        var request = BuildRequest("Salads", ingredients, stations);

        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal("Salads", result.Name);
        Assert.Equal(2, result.IngredientIds.Count);
        Assert.Equal(2, result.StationIds.Count);
        Assert.Contains(1u, result.IngredientIds);
        Assert.Contains(10u, result.StationIds);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyLists_PersistsEmptyLists()
    {
        var request = BuildRequest("Empty Category", new List<uint>(), new List<uint>());
        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Empty(result.IngredientIds);
        Assert.Empty(result.StationIds);
    }

    [Fact]
    public async Task CreateAsync_IncreasesCount()
    {
        await CreateCategoryAsync("A");
        await CreateCategoryAsync("B");

        var count = await GetAllCountAsync();
        Assert.Equal(2, count);
    }

    // ─── Update ────────────────────────────────────────────────
    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesCategory()
    {
        var created = await CreateCategoryAsync("Old Name", new List<uint> { 1 }, new List<uint> { 2 });

        var updateRequest = BuildRequest(
            name: "New Name",
            ingredientIds: new List<uint> { 3, 4 },
            stationIds: new List<uint> { 5 }
        );

        var success = await _service.UpdateAsync(created.Id, updateRequest);
        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal(2, updated.IngredientIds.Count);
        Assert.Contains(3u, updated.IngredientIds);
        Assert.Single(updated.StationIds);
        Assert.Contains(5u, updated.StationIds);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());
        Assert.False(success);
    }

    // ─── Delete ────────────────────────────────────────────────
    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesCategory()
    {
        var created = await CreateCategoryAsync("Category X");

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
        var a = await CreateCategoryAsync("A");
        var b = await CreateCategoryAsync("B");

        await _service.DeleteAsync(a.Id);

        var all = await _service.GetAllAsync();
        Assert.Single(all);
        Assert.Equal(b.Id, all[0].Id);
    }
}