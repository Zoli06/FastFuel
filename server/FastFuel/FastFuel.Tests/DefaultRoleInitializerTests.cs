using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Permissions.Services;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Roles.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastFuel.Tests;

public class DefaultRoleInitializerTests : IAsyncLifetime, IClassFixture<MariaDbFixture>
{
    private readonly MariaDbFixture _fixture;

    private FastFuelDbContext _dbContext = null!;
    private RoleManager<Role> _roleManager = null!;
    private TestPermissionService _permissionService = null!;
    private DefaultRoleInitializer _initializer = null!;

    public DefaultRoleInitializerTests(MariaDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();
        _roleManager = CreateRoleManager();
        _permissionService = new TestPermissionService();
        _initializer = new DefaultRoleInitializer(_roleManager, _permissionService);

        await CleanupDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupDatabaseAsync();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task InitializeAsync_ShouldCreateMissingDefaultRolesWithoutOverwritingExistingOnes()
    {
        var existingCustomer = new Role
        {
            Name = nameof(DefaultRole.Customer),
            IsDefault = false,
            ArePermissionsImmutable = true
        };

        await _roleManager.CreateAsync(existingCustomer);

        _permissionService.SetPermissions(["Permission:Admin:Manage"]);

        await _initializer.InitializeAsync();

        var roleNames = await _dbContext.Roles.Select(r => r.Name).ToListAsync();
        Assert.Contains(nameof(DefaultRole.Admin), roleNames);
        Assert.Contains(nameof(DefaultRole.Customer), roleNames);
        Assert.Contains(nameof(DefaultRole.Employee), roleNames);
        Assert.Contains(nameof(DefaultRole.Machine), roleNames);

        var customerRole = await _roleManager.FindByNameAsync(nameof(DefaultRole.Customer));
        Assert.NotNull(customerRole);
        Assert.False(customerRole.IsDefault);
        Assert.True(customerRole.ArePermissionsImmutable);

        Assert.Equal(1, await _dbContext.Roles.CountAsync(r => r.Name == nameof(DefaultRole.Customer)));
    }

    [Fact]
    public async Task InitializeAsync_ShouldReconcileAdminPermissionsOnEveryStart()
    {
        var adminRole = new Role
        {
            Name = nameof(DefaultRole.Admin),
            IsDefault = true,
            ArePermissionsImmutable = true
        };

        await _roleManager.CreateAsync(adminRole);
        await _roleManager.AddClaimAsync(adminRole, new Claim("Permission", "Permission:Legacy"));

        _permissionService.SetPermissions(["Permission:Orders:Read"]);

        await _initializer.InitializeAsync();

        var refreshedAdmin = await _roleManager.FindByNameAsync(nameof(DefaultRole.Admin));
        Assert.NotNull(refreshedAdmin);

        var firstRunPermissions = (await _roleManager.GetClaimsAsync(refreshedAdmin))
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .OrderBy(v => v)
            .ToList();

        Assert.Equal(["Permission:Orders:Read"], firstRunPermissions);

        _permissionService.SetPermissions(["Permission:Menus:Read", "Permission:Menus:Write"]);

        await _initializer.InitializeAsync();

        refreshedAdmin = await _roleManager.FindByNameAsync(nameof(DefaultRole.Admin));
        var secondRunPermissions = (await _roleManager.GetClaimsAsync(refreshedAdmin!))
            .Where(c => c.Type == "Permission")
            .Select(c => c.Value)
            .OrderBy(v => v)
            .ToList();

        Assert.Equal(["Permission:Menus:Read", "Permission:Menus:Write"], secondRunPermissions);
        Assert.Equal(1, await _dbContext.Roles.CountAsync(r => r.Name == nameof(DefaultRole.Admin)));
    }

    private async Task CleanupDatabaseAsync()
    {
        _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles);
        _dbContext.RoleClaims.RemoveRange(_dbContext.RoleClaims);
        _dbContext.Roles.RemoveRange(_dbContext.Roles);
        await _dbContext.SaveChangesAsync();
    }

    private RoleManager<Role> CreateRoleManager()
    {
        var store = new RoleStore<Role, FastFuelDbContext, uint>(_dbContext);

        return new RoleManager<Role>(
            store,
            new List<IRoleValidator<Role>> { new RoleValidator<Role>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new DummyLogger<RoleManager<Role>>()
        );
    }

    private class TestPermissionService : IPermissionService
    {
        private List<string> _permissions = [];

        public void SetPermissions(IEnumerable<string> permissions)
        {
            _permissions = permissions.ToList();
        }

        public Task<List<string>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_permissions.ToList());
        }

        public Task<List<string>> GetPermissionsForCurrentUserAsync(ClaimsPrincipal user)
        {
            return Task.FromResult(new List<string>());
        }
    }

    private class DummyLogger<T> : ILogger<T>
    {
        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
        }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}