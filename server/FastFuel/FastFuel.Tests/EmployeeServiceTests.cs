using System.Security.Claims;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Mappers;
using FastFuel.Features.Employees.Services;
using FastFuel.Features.Permissions.Services;
using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Roles.Services;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastFuel.Tests;

public class EmployeeServiceTests : IAsyncLifetime, IClassFixture<MariaDbFixture>
{
    private readonly MariaDbFixture _fixture;

    private FastFuelDbContext _dbContext = null!;
    private EmployeeService _service = null!;
    private uint _restaurantId;

    public EmployeeServiceTests(MariaDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();

        await ResetDatabaseAsync();

        var userManager = CreateUserManager();
        var roleManager = CreateRoleManager();

        var roleInitializer = new DefaultRoleInitializer(roleManager, new TestPermissionService());
        await roleInitializer.InitializeAsync();

        var restaurant = new Restaurant
        {
            Name = "Test Restaurant",
            Address = "Test Address",
            Latitude = 0,
            Longitude = 0
        };
        _dbContext.Restaurants.Add(restaurant);
        await _dbContext.SaveChangesAsync();
        _restaurantId = restaurant.Id;

        var mapper = new EmployeeMapper(_dbContext, roleManager, userManager);

        _service = new EmployeeService(
            _dbContext,
            mapper,
            userManager
        );
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    private async Task ResetDatabaseAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
    }

    // -------------------------
    // Identity helpers
    // -------------------------

    private UserManager<User> CreateUserManager()
    {
        var store = new UserStore<User, Role, FastFuelDbContext, uint>(_dbContext);

        var options = new OptionsWrapper<IdentityOptions>(new IdentityOptions());

        var logger = new LoggerFactory().CreateLogger<UserManager<User>>();

        return new UserManager<User>(
            store,
            options,
            new PasswordHasher<User>(),
            new List<IUserValidator<User>> { new UserValidator<User>() },
            new List<IPasswordValidator<User>> { new PasswordValidator<User>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            logger
        );
    }

    private RoleManager<Role> CreateRoleManager()
    {
        var store = new RoleStore<Role, FastFuelDbContext, uint>(_dbContext);

        var logger = new LoggerFactory().CreateLogger<RoleManager<Role>>();

        return new RoleManager<Role>(
            store,
            new List<IRoleValidator<Role>> { new RoleValidator<Role>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            logger
        );
    }

    private class TestPermissionService : IPermissionService
    {
        public Task<List<string>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new List<string>());
        }

        public Task<List<string>> GetPermissionsForCurrentUserAsync(ClaimsPrincipal user)
        {
            return Task.FromResult(new List<string>());
        }
    }

    // -------------------------
    // Test
    // -------------------------

    [Fact]
    public async Task CreateEmployee_ShouldCreateEmployee()
    {
        var request = new EmployeeRequestDto
        {
            Name = "Test Employee",
            UserName = "employee",
            Email = "employee@test.com",
            Password = "Password123!",
            ShiftIds = new List<uint>(),
            StationCategoryIds = new List<uint>(),
            WorksAtRestaurantId = _restaurantId
        };

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
    }
}