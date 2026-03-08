using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.Shifts.DTOs;
using FastFuel.Features.Shifts.Mappers;
using FastFuel.Features.Shifts.Services;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Tests;

public class ShiftServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private ShiftService _service = null!;
    private Employee _defaultEmployee = null!;

    // ─── Lifecycle ───────────────────────────────────────────────

    public async Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        _service = new ShiftService(_dbContext, new ShiftMapper());

        // Seed a fully valid employee for tests
        _defaultEmployee = new Employee
        {
            Name = "Test Employee",
            UserName = "testemployee",
            NormalizedUserName = "TESTEMPLOYEE",
            Email = "test@example.com",
            NormalizedEmail = "TEST@EXAMPLE.COM",
            EmailConfirmed = true
        };

        // Set a password hash to satisfy non-null constraint
        var hasher = new PasswordHasher<Employee>();
        _defaultEmployee.PasswordHash = hasher.HashPassword(_defaultEmployee, "TestPassword123!");

        _dbContext.Employees.Add(_defaultEmployee);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        // Clean up all shifts and employees after tests
        _dbContext.Shifts.RemoveRange(_dbContext.Shifts);
        _dbContext.Employees.RemoveRange(_dbContext.Employees);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ────────────────────────────────────────────────

    private ShiftRequestDto BuildRequest(
        DateTime? startTime = null,
        DateTime? endTime = null,
        uint? employeeId = null)
    {
        return new ShiftRequestDto
        {
            StartTime = startTime ?? DateTime.UtcNow,
            EndTime = endTime ?? DateTime.UtcNow.AddHours(8),
            EmployeeId = employeeId ?? _defaultEmployee.Id
        };
    }

    private async Task<List<ShiftResponseDto>> CreateShiftsAsync(int count)
    {
        var shifts = new List<ShiftResponseDto>();
        for (int i = 0; i < count; i++)
        {
            shifts.Add(await _service.CreateAsync(BuildRequest()));
        }
        return shifts;
    }

    // ─── GetAll ─────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        var result = await _service.GetAllAsync();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenShiftExists_ReturnsThatShift()
    {
        await _service.CreateAsync(BuildRequest());

        var result = await _service.GetAllAsync();
        Assert.Single(result);
        Assert.Equal(_defaultEmployee.Id, result[0].EmployeeId);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCreatedShifts()
    {
        await _service.CreateAsync(BuildRequest());
        await _service.CreateAsync(BuildRequest());
        await _service.CreateAsync(BuildRequest());

        var result = await _service.GetAllAsync();
        Assert.Equal(3, result.Count);
    }

    // ─── GetById ────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsCorrectShift()
    {
        var created = await _service.CreateAsync(BuildRequest());

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(_defaultEmployee.Id, result.EmployeeId);
        Assert.Equal(created.StartTime, result.StartTime);
        Assert.Equal(created.EndTime, result.EndTime);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);
        Assert.Null(result);
    }

    // ─── Create ─────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_PersistsShift()
    {
        var request = BuildRequest();
        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal(_defaultEmployee.Id, result.EmployeeId);
        Assert.Equal(request.StartTime, result.StartTime);
        Assert.Equal(request.EndTime, result.EndTime);
    }

    [Fact]
    public async Task CreateAsync_WithCustomTimes_PersistsCorrectTimes()
    {
        var start = DateTime.UtcNow;
        var end = start.AddHours(12);

        var result = await _service.CreateAsync(BuildRequest(start, end));

        Assert.Equal(start, result.StartTime);
        Assert.Equal(end, result.EndTime);
        Assert.Equal(_defaultEmployee.Id, result.EmployeeId);
    }

    [Fact]
    public async Task CreateAsync_IncreasesCount()
    {
        await _service.CreateAsync(BuildRequest());
        await _service.CreateAsync(BuildRequest());

        var all = await _service.GetAllAsync();
        Assert.Equal(2, all.Count);
    }

    // ─── Update ─────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesAndReturnsTrue()
    {
        var created = await _service.CreateAsync(BuildRequest());
        var updateRequest = BuildRequest();

        var success = await _service.UpdateAsync(created.Id, updateRequest);

        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Equal(_defaultEmployee.Id, updated!.EmployeeId);
        Assert.Equal(updateRequest.StartTime, updated.StartTime);
        Assert.Equal(updateRequest.EndTime, updated.EndTime);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());
        Assert.False(success);
    }

    // ─── Delete ─────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesAndReturnsTrue()
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
    public async Task DeleteAsync_DecreasesCount()
    {
        var s1 = await _service.CreateAsync(BuildRequest());
        var s2 = await _service.CreateAsync(BuildRequest());

        await _service.DeleteAsync(s1.Id);

        var all = await _service.GetAllAsync();
        Assert.Single(all);
        Assert.Equal(s2.Id, all[0].Id);
    }
}