using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Themes.DTOs;
using FastFuel.Features.Themes.Mappers;
using FastFuel.Features.Themes.Services;

namespace FastFuel.Tests;

public class ThemeServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private ThemeService _service = null!;

    // ─── Lifecycle ─────────────────────────────────────────────
    public Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        var mapper = new ThemeMapper();
        _service = new ThemeService(_dbContext, mapper);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _dbContext.Themes.RemoveRange(_dbContext.Themes);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────
    private static ThemeRequestDto BuildRequest(
        string name = "Default Theme",
        string background = "#FFFFFF",
        string footer = "#000000",
        string buttonPrimary = "#FF0000",
        string buttonSecondary = "#00FF00")
    {
        return new ThemeRequestDto
        {
            Name = name,
            Background = background,
            Footer = footer,
            ButtonPrimary = buttonPrimary,
            ButtonSecondary = buttonSecondary
        };
    }

    private async Task<ThemeResponseDto> CreateThemeAsync(
        string name = "Default Theme",
        string background = "#FFFFFF",
        string footer = "#000000",
        string buttonPrimary = "#FF0000",
        string buttonSecondary = "#00FF00")
    {
        return await _service.CreateAsync(BuildRequest(name, background, footer, buttonPrimary, buttonSecondary));
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
    public async Task GetAllAsync_WhenThemesExist_ReturnsAll()
    {
        await CreateThemeAsync("Theme A");
        await CreateThemeAsync("Theme B");
        await CreateThemeAsync("Theme C");

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, t => t.Name == "Theme A");
        Assert.Contains(result, t => t.Name == "Theme B");
        Assert.Contains(result, t => t.Name == "Theme C");
    }

    // ─── GetById ───────────────────────────────────────────────
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsTheme()
    {
        var created = await CreateThemeAsync("My Theme");

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("My Theme", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);
        Assert.Null(result);
    }

    // ─── Create ────────────────────────────────────────────────
    [Fact]
    public async Task CreateAsync_PersistsTheme()
    {
        var request = BuildRequest(
            name: "Dark Theme",
            background: "#111111",
            footer: "#222222",
            buttonPrimary: "#333333",
            buttonSecondary: "#444444"
        );

        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal("Dark Theme", result.Name);
        Assert.Equal("#111111", result.Background);
        Assert.Equal("#222222", result.Footer);
        Assert.Equal("#333333", result.ButtonPrimary);
        Assert.Equal("#444444", result.ButtonSecondary);
    }

    [Fact]
    public async Task CreateAsync_IncreasesCount()
    {
        await CreateThemeAsync("Theme 1");
        await CreateThemeAsync("Theme 2");

        var count = await GetAllCountAsync();
        Assert.Equal(2, count);
    }

    // ─── Update ────────────────────────────────────────────────
    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesTheme()
    {
        var created = await CreateThemeAsync("Old Theme");

        var updateRequest = BuildRequest(
            name: "New Theme",
            background: "#AAAAAA",
            footer: "#BBBBBB",
            buttonPrimary: "#CCCCCC",
            buttonSecondary: "#DDDDDD"
        );

        var success = await _service.UpdateAsync(created.Id, updateRequest);
        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Equal("New Theme", updated!.Name);
        Assert.Equal("#AAAAAA", updated.Background);
        Assert.Equal("#BBBBBB", updated.Footer);
        Assert.Equal("#CCCCCC", updated.ButtonPrimary);
        Assert.Equal("#DDDDDD", updated.ButtonSecondary);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());
        Assert.False(success);
    }

    // ─── Delete ────────────────────────────────────────────────
    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesTheme()
    {
        var created = await CreateThemeAsync("Theme X");

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
        var a = await CreateThemeAsync("Theme A");
        var b = await CreateThemeAsync("Theme B");

        await _service.DeleteAsync(a.Id);

        var all = await _service.GetAllAsync();
        Assert.Single(all);
        Assert.Equal(b.Id, all[0].Id);
    }
}