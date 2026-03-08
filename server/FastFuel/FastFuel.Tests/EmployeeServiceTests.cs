using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Mappers;
using FastFuel.Features.Employees.Services;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using FastFuel.Tests;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class EmployeeServiceTests : IAsyncLifetime, IClassFixture<MariaDbFixture>
{
    private readonly MariaDbFixture _fixture;

    private ApplicationDbContext _dbContext = null!;
    private EmployeeService _service = null!;

    public EmployeeServiceTests(MariaDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();

        var userManager = CreateUserManager();
        var roleManager = CreateRoleManager();

        var mapper = new EmployeeMapper(_dbContext, roleManager, userManager);

        _service = new EmployeeService(
            _dbContext,
            mapper,
            userManager,
            roleManager
        );

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }

    // -------------------------
    // Identity helpers
    // -------------------------

    private UserManager<User> CreateUserManager()
    {
        var store = new UserStore<User, Role, ApplicationDbContext, uint>(_dbContext);

        return new UserManager<User>(
            store,
            null,
            new PasswordHasher<User>(),
            new List<IUserValidator<User>>(),
            new List<IPasswordValidator<User>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            null
        );
    }

    private RoleManager<Role> CreateRoleManager()
    {
        var store = new RoleStore<Role, ApplicationDbContext, uint>(_dbContext);

        return new RoleManager<Role>(
            store,
            new List<IRoleValidator<Role>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null
        );
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
            ThemeId = null,
            ShiftIds = [],
            StationCategoryIds = []
        };

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
    }
}