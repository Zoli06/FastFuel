using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Roles.Mappers;
using FastFuel.Features.Roles.Services;
using FastFuel.Features.Users.Entities;
using FastFuel.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

public class RoleServiceTests : IAsyncLifetime, IClassFixture<MariaDbFixture>
{
    private readonly MariaDbFixture _fixture;

    private ApplicationDbContext _dbContext = null!;
    private RoleService _service = null!;
    private RoleManager<Role> _roleManager = null!;
    private UserManager<User> _userManager = null!;

    public RoleServiceTests(MariaDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();

        _userManager = CreateUserManager();
        _roleManager = CreateRoleManager();

        var mapper = new RoleMapper(_roleManager, _userManager);

        _service = new RoleService(
            _dbContext,
            mapper,
            _roleManager,
            _userManager
        );

        // Ensure clean DB
        await CleanupDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupDatabaseAsync();
        await _dbContext.DisposeAsync();
    }

    private async Task CleanupDatabaseAsync()
    {
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.Roles.RemoveRange(_dbContext.Roles);
        await _dbContext.SaveChangesAsync();
    }

    // -------------------------
    // Identity Helpers
    // -------------------------

    private class DummyLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
        private class NullScope : IDisposable { public static readonly NullScope Instance = new NullScope(); public void Dispose() { } }
    }

    private UserManager<User> CreateUserManager()
    {
        var store = new UserStore<User, Role, ApplicationDbContext, uint>(_dbContext);

        return new UserManager<User>(
            store,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<User>(),
            new List<IUserValidator<User>> { new UserValidator<User>() },
            new List<IPasswordValidator<User>> { new PasswordValidator<User>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new DummyServiceProvider(),
            new DummyLogger<UserManager<User>>()
        );
    }

    private RoleManager<Role> CreateRoleManager()
    {
        var store = new RoleStore<Role, ApplicationDbContext, uint>(_dbContext);

        return new RoleManager<Role>(
            store,
            new List<IRoleValidator<Role>> { new RoleValidator<Role>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new DummyLogger<RoleManager<Role>>()
        );
    }

    private class DummyServiceProvider : IServiceProvider { public object? GetService(Type serviceType) => null; }

    // -------------------------
    // Helpers
    // -------------------------

    private static RoleRequestDto BuildRequest(
        string name = "TestRole",
        List<string>? permissions = null,
        List<uint>? userIds = null)
    {
        return new RoleRequestDto
        {
            Name = name,
            Permissions = permissions ?? new List<string>(),
            UserIds = userIds ?? new List<uint>()
        };
    }

    // -------------------------
    // Tests
    // -------------------------

    [Fact]
    public async Task CreateRole_ShouldCreateRole()
    {
        var request = BuildRequest("Manager");

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal("Manager", result.Name);
    }

    [Fact]
    public async Task CreateRole_WithPermissions_ShouldAddClaims()
    {
        var request = BuildRequest(
            "Cashier",
            new List<string> { "Permission:Order:Read", "Permission:Order:Create" }
        );

        var result = await _service.CreateAsync(request);

        var role = await _roleManager.FindByNameAsync(result.Name);
        var claims = await _roleManager.GetClaimsAsync(role!);

        Assert.Equal(2, claims.Count);
        Assert.Contains(claims, c => c.Value == "Permission:Order:Read");
        Assert.Contains(claims, c => c.Value == "Permission:Order:Create");
    }

    [Fact]
    public async Task UpdateRole_ShouldUpdatePermissions()
    {
        var created = await _service.CreateAsync(
            BuildRequest("Supervisor", new List<string> { "Permission:A" })
        );

        var updateRequest = BuildRequest(
            "Supervisor",
            new List<string> { "Permission:B" }
        );

        await _service.UpdateAsync(created.Id, updateRequest);

        var role = await _roleManager.FindByNameAsync("Supervisor");
        var claims = await _roleManager.GetClaimsAsync(role!);

        Assert.Single(claims);
        Assert.Equal("Permission:B", claims[0].Value);
    }

    [Fact]
    public async Task Update_DefaultRole_ShouldThrowException()
    {
        var role = new Role
        {
            Name = "DefaultRole",
            IsDefault = true
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        var request = BuildRequest("DefaultRole", new List<string> { "Permission:Test" });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.UpdateAsync(role.Id, request)
        );
    }

    [Fact]
    public async Task DeleteRole_ShouldRemoveRole()
    {
        var created = await _service.CreateAsync(BuildRequest("TempRole"));

        var success = await _service.DeleteAsync(created.Id);

        Assert.True(success);

        var role = await _roleManager.FindByNameAsync("TempRole");
        Assert.Null(role);
    }

    [Fact]
    public async Task Delete_DefaultRole_ShouldThrowException()
    {
        var role = new Role
        {
            Name = "ProtectedRole",
            IsDefault = true
        };

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.DeleteAsync(role.Id)
        );
    }
}