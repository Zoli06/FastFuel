using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Roles.Mappers;
using FastFuel.Features.Roles.Services;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastFuel.Tests;

public class RoleServiceTests : IAsyncLifetime, IClassFixture<MariaDbFixture>
{
    private readonly MariaDbFixture _fixture;

    private ApplicationDbContext _dbContext = null!;
    private RoleManager<Role> _roleManager = null!;
    private RoleService _service = null!;
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
    public async Task Update_ImmutableRole_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = "Admin",
            IsDefault = true,
            IsImmutable = true
        };

        await _roleManager.CreateAsync(role);

        var request = BuildRequest("RenamedAdmin", new List<string> { "Permission:Test" });

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.UpdateAsync(role.Id, request)
        );
    }

    [Fact]
    public async Task Update_DefaultRole_WhenMutable_ShouldUpdatePermissions()
    {
        var role = new Role
        {
            Name = "DefaultRole",
            IsDefault = true,
            IsImmutable = false
        };

        await _roleManager.CreateAsync(role);

        var request = BuildRequest("DefaultRole", new List<string> { "Permission:Test" });

        var updated = await _service.UpdateAsync(role.Id, request);

        Assert.True(updated);

        var refreshedRole = await _roleManager.FindByNameAsync("DefaultRole");
        var claims = await _roleManager.GetClaimsAsync(refreshedRole!);

        Assert.Single(claims);
        Assert.Equal("Permission:Test", claims[0].Value);
    }

    [Fact]
    public async Task Update_DefaultRole_WhenRenaming_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = "Customer",
            IsDefault = true,
            IsImmutable = false
        };

        await _roleManager.CreateAsync(role);

        var request = BuildRequest("RenamedCustomer", new List<string> { "Permission:Test" });

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.UpdateAsync(role.Id, request)
        );
    }

    [Fact]
    public async Task Update_DefaultRole_WhenAddingUser_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = "Customer",
            IsDefault = true,
            IsImmutable = false
        };

        await _roleManager.CreateAsync(role);

        var user = new User
        {
            UserName = "default-role-add-user@test.local",
            Email = "default-role-add-user@test.local"
        };
        await _userManager.CreateAsync(user);

        var request = BuildRequest("Customer", new List<string>(), new List<uint> { user.Id });

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.UpdateAsync(role.Id, request)
        );

        Assert.False(await _userManager.IsInRoleAsync(user, role.Name));
    }

    [Fact]
    public async Task Update_DefaultRole_WhenRemovingUser_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = "Customer",
            IsDefault = true,
            IsImmutable = false
        };

        await _roleManager.CreateAsync(role);

        var user = new User
        {
            UserName = "default-role-remove-user@test.local",
            Email = "default-role-remove-user@test.local"
        };
        await _userManager.CreateAsync(user);
        await _userManager.AddToRoleAsync(user, role.Name);

        var request = BuildRequest("Customer");

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.UpdateAsync(role.Id, request)
        );

        Assert.True(await _userManager.IsInRoleAsync(user, role.Name));
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
    public async Task Delete_ImmutableRole_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = "Admin",
            IsDefault = true,
            IsImmutable = true
        };

        await _roleManager.CreateAsync(role);

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.DeleteAsync(role.Id)
        );
    }

    [Fact]
    public async Task Delete_DefaultRole_WhenMutable_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = "Customer",
            IsDefault = true,
            IsImmutable = false
        };

        await _roleManager.CreateAsync(role);

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.DeleteAsync(role.Id)
        );
    }

    // -------------------------
    // Identity Helpers
    // -------------------------

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

    private class DummyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return null;
        }
    }
}