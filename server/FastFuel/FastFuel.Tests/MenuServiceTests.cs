using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Menus.Mappers;
using FastFuel.Features.Menus.Services;

namespace FastFuel.Tests;

public class MenuServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private MenuService _service = null!;

    private static readonly Uri DefaultImage =
        new("https://test.com/menu.png");

    // ─── Lifecycle ─────────────────────────────────────────────

    public Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        var mapper = new MenuMapper();
        _service = new MenuService(_dbContext, mapper);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _dbContext.Menus.RemoveRange(_dbContext.Menus);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────

    private static MenuRequestDto BuildRequest(
        string name = "Test Menu",
        uint price = 1000,
        string? description = "Test description",
        Uri? imageUrl = null,
        List<MenuFoodDto>? foods = null)
    {
        return new MenuRequestDto
        {
            Name = name,
            Price = price,
            Description = description,
            ImageUrl = imageUrl,
            Foods = foods ?? []
        };
    }

    private async Task<MenuResponseDto> CreateMenuAsync(
        string name = "Test Menu",
        uint price = 1000)
    {
        return await _service.CreateAsync(BuildRequest(name, price));
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
    public async Task GetAllAsync_WhenMenusExist_ReturnsAll()
    {
        await CreateMenuAsync("Menu A");
        await CreateMenuAsync("Menu B");
        await CreateMenuAsync("Menu C");

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, m => m.Name == "Menu A");
        Assert.Contains(result, m => m.Name == "Menu B");
        Assert.Contains(result, m => m.Name == "Menu C");
    }

    // ─── GetById ───────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsMenu()
    {
        var created = await CreateMenuAsync("Burger Menu");

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Burger Menu", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);

        Assert.Null(result);
    }

    // ─── Create ────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_PersistsMenu()
    {
        var request = BuildRequest(
            name: "Burger Menu",
            price: 1500,
            description: "Burger with fries",
            imageUrl: DefaultImage
        );

        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal("Burger Menu", result.Name);
        Assert.Equal(1500u, result.Price);
        Assert.Equal("Burger with fries", result.Description);
        Assert.Equal(DefaultImage, result.ImageUrl);
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
        await CreateMenuAsync("Menu A");
        await CreateMenuAsync("Menu B");

        var count = await GetAllCountAsync();

        Assert.Equal(2, count);
    }

    // ─── Update ────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesMenu()
    {
        var created = await CreateMenuAsync("Old Menu");

        var updateRequest = BuildRequest(
            name: "New Menu",
            price: 2000,
            description: "Updated description",
            imageUrl: new Uri("https://test.com/new.png")
        );

        var success = await _service.UpdateAsync(created.Id, updateRequest);

        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);

        Assert.Equal("New Menu", updated!.Name);
        Assert.Equal((uint)2000, updated.Price);
        Assert.Equal("Updated description", updated.Description);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());

        Assert.False(success);
    }

    // ─── Delete ────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesMenu()
    {
        var created = await CreateMenuAsync("Menu A");

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
        var a = await CreateMenuAsync("Menu A");
        var b = await CreateMenuAsync("Menu B");

        await _service.DeleteAsync(a.Id);

        var all = await _service.GetAllAsync();

        Assert.Single(all);
        Assert.Equal(b.Id, all[0].Id);
    }
}