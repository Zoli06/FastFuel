using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Shifts.DTOs;
using FastFuel.Features.Shifts.Mappers;
using FastFuel.Features.Shifts.Services;

namespace FastFuel.Tests;

public class ShiftServiceTests(MariaDbFixture fixture)
    : IClassFixture<MariaDbFixture>, IAsyncLifetime
{
    private ApplicationDbContext _dbContext = null!;
    private ShiftService _service = null!;

    // ─── Lifecycle ─────────────────────────────────────────────
    public Task InitializeAsync()
    {
        _dbContext = fixture.CreateDbContext();
        var mapper = new ShiftMapper();
        _service = new ShiftService(_dbContext, mapper);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _dbContext.Shifts.RemoveRange(_dbContext.Shifts);
        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // ─── Helpers ───────────────────────────────────────────────
    private static ShiftRequestDto BuildRequest(
        DateTime? startTime = null,
        DateTime? endTime = null,
        uint employeeId = 1)
    {
        return new ShiftRequestDto
        {
            StartTime = startTime ?? DateTime.Today.AddHours(9),
            EndTime = endTime ?? DateTime.Today.AddHours(17),
            EmployeeId = employeeId
        };
    }

    private async Task<ShiftResponseDto> CreateShiftAsync(
        uint employeeId = 1,
        DateTime? startTime = null,
        DateTime? endTime = null)
    {
        return await _service.CreateAsync(BuildRequest(startTime, endTime, employeeId));
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
    public async Task GetAllAsync_WhenShiftsExist_ReturnsAll()
    {
        await CreateShiftAsync(employeeId: 1);
        await CreateShiftAsync(employeeId: 2);
        await CreateShiftAsync(employeeId: 3);

        var result = await _service.GetAllAsync();

        Assert.Equal(3, result.Count);
        Assert.Contains(result, s => s.EmployeeId == 1);
        Assert.Contains(result, s => s.EmployeeId == 2);
        Assert.Contains(result, s => s.EmployeeId == 3);
    }

    // ─── GetById ───────────────────────────────────────────────
    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsShift()
    {
        var created = await CreateShiftAsync(employeeId: 42);

        var result = await _service.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(42u, result.EmployeeId);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(99999);
        Assert.Null(result);
    }

    // ─── Create ────────────────────────────────────────────────
    [Fact]
    public async Task CreateAsync_PersistsShift()
    {
        var start = DateTime.Today.AddHours(8);
        var end = DateTime.Today.AddHours(16);

        var request = BuildRequest(startTime: start, endTime: end, employeeId: 10);

        var result = await _service.CreateAsync(request);

        Assert.NotEqual(0u, result.Id);
        Assert.Equal(start, result.StartTime);
        Assert.Equal(end, result.EndTime);
        Assert.Equal(10u, result.EmployeeId);
    }

    [Fact]
    public async Task CreateAsync_IncreasesCount()
    {
        await CreateShiftAsync(employeeId: 1);
        await CreateShiftAsync(employeeId: 2);

        var count = await GetAllCountAsync();

        Assert.Equal(2, count);
    }

    // ─── Update ────────────────────────────────────────────────
    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesShift()
    {
        var created = await CreateShiftAsync(employeeId: 5);

        var newStart = DateTime.Today.AddHours(10);
        var newEnd = DateTime.Today.AddHours(18);

        var updateRequest = BuildRequest(startTime: newStart, endTime: newEnd, employeeId: 5);

        var success = await _service.UpdateAsync(created.Id, updateRequest);
        Assert.True(success);

        var updated = await _service.GetByIdAsync(created.Id);
        Assert.Equal(newStart, updated!.StartTime);
        Assert.Equal(newEnd, updated.EndTime);
        Assert.Equal(5u, updated.EmployeeId);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsFalse()
    {
        var success = await _service.UpdateAsync(99999, BuildRequest());
        Assert.False(success);
    }

    // ─── Delete ────────────────────────────────────────────────
    [Fact]
    public async Task DeleteAsync_WithValidId_RemovesShift()
    {
        var created = await CreateShiftAsync();

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
        var a = await CreateShiftAsync(employeeId: 1);
        var b = await CreateShiftAsync(employeeId: 2);

        await _service.DeleteAsync(a.Id);

        var all = await _service.GetAllAsync();

        Assert.Single(all);
        Assert.Equal(b.Id, all[0].Id);
    }
}