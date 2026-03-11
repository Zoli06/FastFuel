using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Mappers;
using FastFuel.Features.Employees.Services;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastFuel.Tests;

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

        // Seed required role if your service uses it
        if (!await roleManager.RoleExistsAsync("Employee"))
            await roleManager.CreateAsync(new Role
            {
                Name = "Employee",
                NormalizedName = "EMPLOYEE"
            });

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

    // -------------------------
    // Identity helpers
    // -------------------------

    private UserManager<User> CreateUserManager()
    {
        var store = new UserStore<User, Role, ApplicationDbContext, uint>(_dbContext);

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
        var store = new RoleStore<Role, ApplicationDbContext, uint>(_dbContext);

        var logger = new LoggerFactory().CreateLogger<RoleManager<Role>>();

        return new RoleManager<Role>(
            store,
            new List<IRoleValidator<Role>> { new RoleValidator<Role>() },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            logger
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
            ShiftIds = new List<uint>(),
            StationCategoryIds = new List<uint>()
        };

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
    }
}