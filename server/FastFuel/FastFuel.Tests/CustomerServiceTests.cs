using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Mappers;
using FastFuel.Features.Customers.Services;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using FastFuel.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        // Cleanup database
        _dbContext.Customers.RemoveRange(_dbContext.Customers);
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.Roles.RemoveRange(_dbContext.Roles);

        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
    }

    // -------------------------
    // Identity helpers
    // -------------------------

    private class DummyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

    private class DummyLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new NullScope();
            public void Dispose() { }
        }
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
    // Tests
    // -------------------------

    [Fact]
    public async Task CreateCustomer_ShouldCreateCustomer()
    {
        var request = new CustomerRequestDto
        {
            Name = "Test Customer",
            Email = "test@test.com",
            UserName = "TestUser",
            Password = "Password123!",
            ThemeId = null
        };

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Name, result.Name);
    }

    [Fact]
    public async Task CreateCustomer_ShouldFailWithDuplicateEmail()
    {
        var request1 = new CustomerRequestDto
        {
            Name = "Customer 1",
            UserName = "User1",
            Email = "dup@test.com",
            Password = "Password123!",
            ThemeId = null
        };
        var request2 = new CustomerRequestDto
        {
            Name = "Customer 2",
            UserName = "User2",
            Email = "dup@test.com",
            Password = "Password123!",
            ThemeId = null
        };

        await _service.CreateAsync(request1);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _service.CreateAsync(request2);
        });
    }

    [Fact]
    public async Task GetAllCustomers_ShouldReturnCreatedCustomer()
    {
        var request = new CustomerRequestDto
        {
            Name = "All Test",
            UserName = "AllUser",
            Email = "all@test.com",
            Password = "Password123!",
            ThemeId = null
        };

        await _service.CreateAsync(request);

        var allCustomers = await _service.GetAllAsync();

        Assert.Single(allCustomers);
        Assert.Equal("all@test.com", allCustomers[0].Email);
    }
}