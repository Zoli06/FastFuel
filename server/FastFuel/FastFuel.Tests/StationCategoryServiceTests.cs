using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Ingredients.Entities;
using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.StationCategories.Mappers;
using FastFuel.Features.StationCategories.Services;
using FastFuel.Features.Stations.Entities;

namespace FastFuel.Tests;

public class StationCategoryServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private StationCategoryService _service = null!;
    private Restaurant _defaultRestaurant = null!;

    // ─── Lifecycle ─────────────────────────────────────────────
    public async Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        _service = new StationCategoryService(_dbContext, new StationCategoryMapper(_dbContext));

        _defaultRestaurant = new Restaurant { Name = "Default Restaurant" };
        _dbContext.Restaurants.Add(_defaultRestaurant);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _dbContext.StationCategories.RemoveRange(_dbContext.StationCategories);
        _dbContext.Stations.RemoveRange(_dbContext.Stations);
        _dbContext.Ingredients.RemoveRange(_dbContext.Ingredients);
        _dbContext.Restaurants.RemoveRange(_dbContext.Restaurants);

        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────
    private async Task<Ingredient> SeedIngredientAsync(string name = "Ingredient")
    {
        var ingredient = new Ingredient { Name = name };
        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync();
        return ingredient;
    }

    private async Task<Station> SeedStationAsync(string name = "Station", uint stationCategoryId = 1)
    {
        var station = new Station
        {
            Name = name,
            RestaurantId = _defaultRestaurant.Id,
            StationCategoryId = stationCategoryId
        };
        _dbContext.Stations.Add(station);
        await _dbContext.SaveChangesAsync();
        return station;
    }

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

    // ─── Tests ────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_PersistsCategoryWithRelations()
    {
        var ing1 = await SeedIngredientAsync("Lettuce");
        var ing2 = await SeedIngredientAsync("Tomato");

        var category = await CreateCategoryAsync("Salads", new List<uint> { ing1.Id, ing2.Id });

        var st1 = await SeedStationAsync("Station 1", category.Id);
        var st2 = await SeedStationAsync("Station 2", category.Id);

        var updatedCategory = await _service.GetByIdAsync(category.Id);

        Assert.NotNull(updatedCategory);
        Assert.Equal("Salads", updatedCategory.Name);
        Assert.Equal(2, updatedCategory.IngredientIds.Count);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyLists_PersistsCategory()
    {
        var category = await CreateCategoryAsync("Empty Category", new List<uint>(), new List<uint>());

        Assert.NotNull(category);
        Assert.Empty(category.IngredientIds);
        Assert.Empty(category.StationIds);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        var a = await CreateCategoryAsync("A");
        var b = await CreateCategoryAsync("B");

        var all = await _service.GetAllAsync();
        Assert.Equal(2, all.Count);
        Assert.Contains(all, c => c.Name == "A");
        Assert.Contains(all, c => c.Name == "B");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCategory()
    {
        var category = await CreateCategoryAsync("My Category");
        var result = await _service.GetByIdAsync(category.Id);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("My Category", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesCategory()
    {
        var category = await CreateCategoryAsync("Old Name");
        var ing = await SeedIngredientAsync("New Ingredient");
        var st = await SeedStationAsync("New Station", category.Id);

        var updateRequest = BuildRequest("New Name", new List<uint> { ing.Id }, new List<uint> { st.Id });
        var success = await _service.UpdateAsync(category.Id, updateRequest);

        var updated = await _service.GetByIdAsync(category.Id);

        Assert.True(success);
        Assert.Equal("New Name", updated!.Name);
        Assert.Single(updated.IngredientIds);
        Assert.Single(updated.StationIds);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());
        Assert.False(success);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesCategory()
    {
        var category = await CreateCategoryAsync("ToDelete");
        var success = await _service.DeleteAsync(category.Id);

        var all = await _service.GetAllAsync();

        Assert.True(success);
        Assert.DoesNotContain(all, c => c.Id == category.Id);
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