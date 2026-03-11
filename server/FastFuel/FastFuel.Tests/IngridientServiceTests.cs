using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Ingredients.Mappers;
using FastFuel.Features.Ingredients.Services;

namespace FastFuel.Tests;

public class IngredientServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private IngredientService _service = null!;

    // ─── Lifecycle ──────────────────────────────────────────────────────────────

    public Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        var mapper = new IngredientMapper(_dbContext);
        _service = new IngredientService(_dbContext, mapper);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _dbContext.Ingredients.RemoveRange(_dbContext.Ingredients);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────

    private static IngredientRequestDto BuildRequest(
        string name = "TestIngredient",
        Uri? imageUrl = null,
        uint? defaultTimer = null,
        List<uint>? allergyIds = null,
        List<uint>? stationCategoryIds = null)
    {
        return new IngredientRequestDto
        {
            Name = name,
            ImageUrl = imageUrl,
            DefaultTimerValueSeconds = defaultTimer ?? 300,
            AllergyIds = allergyIds ?? [],
            StationCategoryIds = stationCategoryIds ?? []
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
    public async Task GetAllAsync_WhenIngredientsExist_ReturnsAll()
    {
        await _service.CreateAsync(BuildRequest("Tomato"));
        await _service.CreateAsync(BuildRequest("Cheese"));
        await _service.CreateAsync(BuildRequest("Bread"));

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, x => x.Name == "Tomato");
        Assert.Contains(result, x => x.Name == "Cheese");
        Assert.Contains(result, x => x.Name == "Bread");
    }

    // ─── GetById ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsIngredient()
    {
        var created = await _service.CreateAsync(BuildRequest("Tomato"));

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Tomato", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);

        Assert.Null(result);
    }

    // ─── Create ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_PersistsIngredient()
    {
        var request = BuildRequest(
            "Tomato",
            new Uri("https://test.com/tomato.png"),
            30
        );

        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal("Tomato", result.Name);
        Assert.Equal(new Uri("https://test.com/tomato.png"), result.ImageUrl);
        Assert.Equal(30u, result.DefaultTimerValueSeconds);
    }

    [Fact]
    public async Task CreateAsync_WithNullImageUrl_PersistsNull()
    {
        var result = await _service.CreateAsync(BuildRequest(imageUrl: null));

        Assert.Null(result.ImageUrl);
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
    public async Task UpdateAsync_WithValidId_UpdatesIngredient()
    {
        var created = await _service.CreateAsync(BuildRequest("OldName"));

        var updateRequest = BuildRequest(
            "NewName",
            new Uri("https://test.com/new.png"),
            60
        );

        var success = await _service.UpdateAsync(created.Id, updateRequest);

        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);

        Assert.Equal("NewName", updated!.Name);
        Assert.Equal(new Uri("https://test.com/new.png"), updated.ImageUrl);
        Assert.Equal(60u, updated.DefaultTimerValueSeconds);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());

        Assert.False(success);
    }

    // ─── Delete ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesIngredient()
    {
        var created = await _service.CreateAsync(BuildRequest("Tomato"));

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
        var a = await _service.CreateAsync(BuildRequest("A"));
        var b = await _service.CreateAsync(BuildRequest("B"));

        await _service.DeleteAsync(a.Id);

        var all = await _service.GetAllAsync();

        Assert.Single(all);
        Assert.Equal(b.Id, all[0].Id);
    }
}