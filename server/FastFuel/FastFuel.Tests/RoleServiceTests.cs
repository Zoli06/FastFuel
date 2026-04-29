using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions.AppExceptions;
using FastFuel.Features.Pages.Common;
using FastFuel.Features.Roles.DTOs;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Roles.Mappers;
using FastFuel.Features.Roles.Services;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastFuel.Tests;

public class RoleServiceTests : IAsyncLifetime, IClassFixture<MariaDbFixture>
{
    private readonly MariaDbFixture _fixture;

    private FastFuelDbContext _dbContext = null!;
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
        var store = new UserStore<User, Role, FastFuelDbContext, uint>(_dbContext);

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
        var store = new RoleStore<Role, FastFuelDbContext, uint>(_dbContext);

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
        List<Page>? pages = null,
        List<uint>? userIds = null)
    {
        return new RoleRequestDto
        {
            Name = name,
            Permissions = permissions ?? new List<string>(),
            Pages = pages ?? new List<Page>(),
            UserIds = userIds ?? new List<uint>()
        };
    }

    private async Task<User> CreateUserAsync(string userName)
    {
        var user = new User
        {
            Name = userName,
            UserName = userName,
            Email = userName
        };

        var createResult = await _userManager.CreateAsync(user, "Password123!");
        if (!createResult.Succeeded)
            throw new Exception(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        return user;
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
    public async Task CreateRole_WithPages_ShouldPersistPages()
    {
        var request = BuildRequest(
            "Kitchen",
            pages: new List<Page> { Page.StationTasks, Page.OrderStatusDisplay, Page.StationTasks }
        );

        var result = await _service.CreateAsync(request);

        Assert.Equal(2, result.Pages.Count);
        Assert.Contains(Page.StationTasks, result.Pages);
        Assert.Contains(Page.OrderStatusDisplay, result.Pages);
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
    public async Task UpdateRole_WithDuplicateExistingPermissionClaims_ShouldNotThrowAndShouldSyncClaims()
    {
        var role = new Role { Name = "SupervisorWithDuplicates" };
        await _roleManager.CreateAsync(role);

        await _roleManager.AddClaimAsync(role, new Claim("Permission", "Permission:A"));
        await _roleManager.AddClaimAsync(role, new Claim("Permission", "Permission:A"));

        var updateRequest = BuildRequest(
            "SupervisorWithDuplicates",
            new List<string> { "Permission:B" }
        );

        var updated = await _service.UpdateAsync(role.Id, updateRequest);

        Assert.True(updated);

        var refreshedRole = await _roleManager.FindByNameAsync("SupervisorWithDuplicates");
        var claims = await _roleManager.GetClaimsAsync(refreshedRole!);

        Assert.Single(claims);
        Assert.Equal("Permission:B", claims[0].Value);
    }

    [Fact]
    public async Task Update_ArePermissionsImmutableRole_WhenChangingPermissions_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = "PermissionLockedRole",
            ArePermissionsImmutable = true
        };

        await _roleManager.CreateAsync(role);
        await _roleManager.AddClaimAsync(role, new Claim("Permission", "Permission:Existing"));

        var request = BuildRequest("PermissionLockedRole", new List<string> { "Permission:Test" });

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.UpdateAsync(role.Id, request)
        );
    }

    [Fact]
    public async Task Update_ArePermissionsImmutableRole_WhenChangingOnlyPages_ShouldSucceed()
    {
        var role = new Role
        {
            Name = "PageLockedPermissions",
            ArePermissionsImmutable = true,
            Pages = new List<Page> { Page.Profile }
        };

        await _roleManager.CreateAsync(role);

        var request = BuildRequest(
            "PageLockedPermissions",
            permissions: new List<string>(),
            pages: new List<Page> { Page.Profile, Page.OrderHistory }
        );

        var updated = await _service.UpdateAsync(role.Id, request);

        Assert.True(updated);

        var refreshedRole = await _dbContext.Roles.FirstAsync(r => r.Id == role.Id);
        Assert.Equal(2, refreshedRole.Pages.Count);
        Assert.Contains(Page.Profile, refreshedRole.Pages);
        Assert.Contains(Page.OrderHistory, refreshedRole.Pages);
    }

    [Fact]
    public async Task Update_DefaultRole_WhenMutable_ShouldUpdatePermissions()
    {
        var role = new Role
        {
            Name = "DefaultRole",
            IsDefault = true,
            ArePermissionsImmutable = false
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
            ArePermissionsImmutable = false
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
            ArePermissionsImmutable = false
        };

        await _roleManager.CreateAsync(role);

        var user = await CreateUserAsync("default-role-add-user@test.local");

        var request = BuildRequest("Customer", new List<string>(), userIds: new List<uint> { user.Id });

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
            ArePermissionsImmutable = false
        };

        await _roleManager.CreateAsync(role);

        var user = await CreateUserAsync("default-role-remove-user@test.local");
        await _userManager.AddToRoleAsync(user, role.Name);

        var request = BuildRequest("Customer");

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.UpdateAsync(role.Id, request)
        );

        Assert.True(await _userManager.IsInRoleAsync(user, role.Name));
    }

    [Fact]
    public async Task Update_AdminRole_WhenRemovingRoleManagerPage_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = nameof(DefaultRole.Admin),
            IsDefault = true,
            ArePermissionsImmutable = false,
            Pages = new List<Page> { Page.RoleManager, Page.Profile }
        };

        await _roleManager.CreateAsync(role);

        var request = BuildRequest(
            nameof(DefaultRole.Admin),
            permissions: new List<string>(),
            pages: new List<Page> { Page.Profile }
        );

        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _service.UpdateAsync(role.Id, request)
        );
    }

    [Fact]
    public async Task GetRolesForCurrentUser_ShouldReturnOnlyCurrentUserRoles()
    {
        var user = await CreateUserAsync("my-roles-user@test.local");

        var cashierRole = new Role { Name = "Cashier" };
        var kitchenRole = new Role { Name = "Kitchen" };
        var managerRole = new Role { Name = "Manager" };

        await _roleManager.CreateAsync(cashierRole);
        await _roleManager.CreateAsync(kitchenRole);
        await _roleManager.CreateAsync(managerRole);

        await _userManager.AddToRoleAsync(user, cashierRole.Name);
        await _userManager.AddToRoleAsync(user, kitchenRole.Name);

        var principal = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        ]));

        var roles = await _service.GetRolesForCurrentUserAsync(principal);
        var roleNames = roles.Select(r => r.Name).ToList();

        Assert.Equal(2, roles.Count);
        Assert.Contains("Cashier", roleNames);
        Assert.Contains("Kitchen", roleNames);
        Assert.DoesNotContain("Manager", roleNames);
    }

    [Fact]
    public async Task GetRolesForCurrentUser_WithoutNameIdentifierClaim_ShouldThrowResourceNotFound()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity());

        await Assert.ThrowsAsync<ResourceNotFoundAppException>(() =>
            _service.GetRolesForCurrentUserAsync(principal)
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
    public async Task Delete_ArePermissionsImmutableNonDefaultRole_ShouldRemoveRole()
    {
        var role = new Role
        {
            Name = "ImmutablePermissionsNonDefault",
            IsDefault = false,
            ArePermissionsImmutable = true
        };

        await _roleManager.CreateAsync(role);

        var deleted = await _service.DeleteAsync(role.Id);

        Assert.True(deleted);

        var refreshedRole = await _roleManager.FindByNameAsync(role.Name);
        Assert.Null(refreshedRole);
    }

    [Fact]
    public async Task Delete_DefaultRole_WhenMutable_ShouldThrowUnauthorizedAppException()
    {
        var role = new Role
        {
            Name = "Customer",
            IsDefault = true,
            ArePermissionsImmutable = false
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