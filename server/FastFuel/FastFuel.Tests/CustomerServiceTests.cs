using EntityFramework.Exceptions.Common;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Mappers;
using FastFuel.Features.Customers.Services;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastFuel.Tests;

public class CustomerServiceTests : IAsyncLifetime, IClassFixture<MariaDbFixture>
{
    private readonly MariaDbFixture _fixture;

    private ApplicationDbContext _dbContext = null!;
    private CustomerService _service = null!;

    public CustomerServiceTests(MariaDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();

        var userManager = CreateUserManager();
        var roleManager = CreateRoleManager();

        var customerMapper = new CustomerMapper(roleManager, userManager);

        _service = new CustomerService(
            _dbContext,
            customerMapper,
            userManager,
            roleManager
        );

        // Ensure database is clean before each test
        await CleanupDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupDatabaseAsync();
        await _dbContext.DisposeAsync();
    }

    private async Task CleanupDatabaseAsync()
    {
        _dbContext.Customers.RemoveRange(_dbContext.Customers);
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

    // ----------------------------------------------------
    // Test Helpers
    // ----------------------------------------------------

    private static CustomerRequestDto BuildRequest(
        string name = "Test Customer",
        string email = "test@test.com",
        string username = "testuser",
        string password = "Password123!")
    {
        return new CustomerRequestDto
        {
            Name = name,
            Email = email,
            UserName = username,
            Password = password,
            ThemeId = null
        };
    }

    // ----------------------------------------------------
    // Tests
    // ----------------------------------------------------

    [Fact]
    public async Task CreateCustomer_ShouldCreateCustomer()
    {
        var request = BuildRequest();

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Name, result.Name);
    }

    [Fact]
    public async Task CreateCustomer_ShouldFailWithDuplicateEmail()
    {
        var request1 = BuildRequest("Customer 1", "dup@test.com", "User1");
        var request2 = BuildRequest("Customer 2", "dup@test.com", "User2");

        await _service.CreateAsync(request1);

        await Assert.ThrowsAsync<UniqueConstraintException>(async () => { await _service.CreateAsync(request2); });
    }

    [Fact]
    public async Task GetAllCustomers_ShouldReturnCreatedCustomer()
    {
        // Ensure database is empty before test
        await CleanupDatabaseAsync();

        var request = BuildRequest("All Test", "all@test.com", "AllUser");

        await _service.CreateAsync(request);

        var allCustomers = await _service.GetAllAsync();

        Assert.Single(allCustomers); // Only 1 customer should exist
        Assert.Equal("all@test.com", allCustomers[0].Email);
    }

    // ----------------------------------------------------
    // Identity Helpers
    // ----------------------------------------------------

    private class DummyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return null;
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