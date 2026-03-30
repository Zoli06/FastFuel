using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Allergies.Mappers;
using FastFuel.Features.Allergies.Services;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Ingredients.Entities;

namespace FastFuel.Tests;

public class AllergyServiceTests(MariaDbFixture fixture) : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private AllergyService _service = null!;

    // ─── Lifecycle ──────────────────────────────────────────────────────────────

    public Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        var mapper = new AllergyMapper(_dbContext);
        _service = new AllergyService(_dbContext, mapper);
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Clean up all allergies and ingredients after every test
        _dbContext.Allergies.RemoveRange(_dbContext.Allergies);
        _dbContext.Ingredients.RemoveRange(_dbContext.Ingredients);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────

    private async Task<Ingredient> SeedIngredientAsync(string name = "TestIngredient")
    {
        var ingredient = new Ingredient { Name = name };
        _dbContext.Ingredients.Add(ingredient);
        await _dbContext.SaveChangesAsync();
        return ingredient;
    }

    private static AllergyRequestDto BuildRequest(
        string name,
        string? message = "Contains gluten",
        List<uint>? ingredientIds = null)
    {
        return new AllergyRequestDto
        {
            Name = name,
            Message = message,
            IngredientIds = ingredientIds ?? []
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
    public async Task GetAllAsync_WhenAllergyExists_ReturnsThatAllergy()
    {
        await _service.CreateAsync(BuildRequest("Peanut", "Nut allergy"));

        var result = await _service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Peanut", result[0].Name);
        Assert.Equal("Nut allergy", result[0].Message);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCreatedAllergies()
    {
        await _service.CreateAsync(BuildRequest("Gluten"));
        await _service.CreateAsync(BuildRequest("Lactose"));
        await _service.CreateAsync(BuildRequest("Soy"));

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, r => r.Name == "Gluten");
        Assert.Contains(result, r => r.Name == "Lactose");
        Assert.Contains(result, r => r.Name == "Soy");
    }

    // ─── GetById ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectAllergy()
    {
        var created = await _service.CreateAsync(BuildRequest("Egg", "Egg allergy"));

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Egg", result.Name);
        Assert.Equal("Egg allergy", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);

        Assert.Null(result);
    }

    // ─── Create ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_WithoutIngredients_PersistsAllergy()
    {
        var request = BuildRequest("Shellfish", "Seafood allergy");

        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal("Shellfish", result.Name);
        Assert.Equal("Seafood allergy", result.Message);
        Assert.Empty(result.IngredientIds);
    }

    [Fact]
    public async Task CreateAsync_WithNullMessage_PersistsNullMessage()
    {
        var request = BuildRequest("Wheat", null);

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task CreateAsync_WithIngredients_LinksIngredients()
    {
        var ingredient = await SeedIngredientAsync("Flour");
        var request = BuildRequest("Gluten", ingredientIds: [ingredient.Id]);

        var result = await _service.CreateAsync(request);

        Assert.Single(result.IngredientIds);
        Assert.Contains(ingredient.Id, result.IngredientIds);
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
    public async Task UpdateAsync_WithValidId_UpdatesAndReturnsTrue()
    {
        var created = await _service.CreateAsync(BuildRequest("OldName", "Old message"));
        var updateRequest = BuildRequest("NewName", "New message");

        var success = await _service.UpdateAsync(created.Id, updateRequest);

        Assert.True(success);
        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Equal("NewName", updated!.Name);
        Assert.Equal("New message", updated.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest("Name"));

        Assert.False(success);
    }

    [Fact]
    public async Task UpdateAsync_ChangesIngredients()
    {
        var ing1 = await SeedIngredientAsync("Oat");
        var ing2 = await SeedIngredientAsync("Barley");
        var created = await _service.CreateAsync(BuildRequest("Gluten", ingredientIds: [ing1.Id]));

        var success = await _service.UpdateAsync(created.Id, BuildRequest("Gluten", ingredientIds: [ing2.Id]));

        Assert.True(success);
        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Single(updated!.IngredientIds);
        Assert.Contains(ing2.Id, updated.IngredientIds);
    }

    [Fact]
    public async Task UpdateAsync_ClearsIngredients_WhenEmptyListProvided()
    {
        var ingredient = await SeedIngredientAsync("Rye");
        var created = await _service.CreateAsync(BuildRequest("Gluten", ingredientIds: [ingredient.Id]));

        await _service.UpdateAsync(created.Id, BuildRequest("Gluten", ingredientIds: []));

        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Empty(updated!.IngredientIds);
    }

    // ─── Delete ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesAndReturnsTrue()
    {
        var created = await _service.CreateAsync(BuildRequest("Soy"));

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
        var a1 = await _service.CreateAsync(BuildRequest("A"));
        var a2 = await _service.CreateAsync(BuildRequest("B"));

        await _service.DeleteAsync(a1.Id);

        var all = await _service.GetAllAsync();
        Assert.Single(all);
        Assert.Equal(a2.Id, all[0].Id);
    }
}