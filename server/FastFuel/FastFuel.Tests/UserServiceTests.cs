using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Users.DTOs;
using FastFuel.Features.Users.Entities;
using FastFuel.Features.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastFuel.Tests;

public class UserServiceTests : IAsyncLifetime, IClassFixture<MariaDbFixture>
{
    private readonly MariaDbFixture _fixture;

    private ApplicationDbContext _dbContext = null!;
    private RoleManager<Role> _roleManager = null!;
    private TestUserService _service = null!;
    private UserManager<User> _userManager = null!;

    public UserServiceTests(MariaDbFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _dbContext = _fixture.CreateDbContext();

        _userManager = CreateUserManager();
        _roleManager = CreateRoleManager();

        var mapper = new TestUserMapper();

        _service = new TestUserService(
            _dbContext,
            mapper,
            _userManager,
            _roleManager
        );

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _dbContext.Users.RemoveRange(_dbContext.Users);
        _dbContext.Roles.RemoveRange(_dbContext.Roles);

        await _dbContext.SaveChangesAsync();
        await _dbContext.DisposeAsync();
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
    // Test Helpers
    // -------------------------

    private static UserRequestDto BuildRequest(
        string name = "Test User",
        string email = "test@test.com",
        string username = "testuser",
        string password = "Password123!")
    {
        return new UserRequestDto
        {
            Name = name,
            Email = email,
            UserName = username,
            Password = password,
            ThemeId = null
        };
    }

    // -------------------------
    // Tests
    // -------------------------

    [Fact]
    public async Task CreateUser_ShouldCreateUser()
    {
        var request = BuildRequest();

        var result = await _service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Email, result.Email);
    }

    [Fact]
    public async Task CreateUser_ShouldCreateDefaultRole()
    {
        await _service.CreateAsync(BuildRequest());

        var role = await _roleManager.FindByNameAsync("User");

        Assert.NotNull(role);
        Assert.True(role.IsDefault);
    }

    [Fact]
    public async Task CreateUser_ShouldAssignDefaultRole()
    {
        var result = await _service.CreateAsync(BuildRequest());

        var user = await _userManager.FindByEmailAsync(result.Email);
        var roles = await _userManager.GetRolesAsync(user!);

        Assert.Contains("User", roles);
    }

    [Fact]
    public async Task CreateUser_WithoutPassword_ShouldThrow()
    {
        var request = new UserRequestDto
        {
            Name = "Test",
            Email = "test@test.com",
            UserName = "test",
            Password = null,
            ThemeId = null
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(request)
        );
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateFields()
    {
        var created = await _service.CreateAsync(BuildRequest());

        var update = new UserRequestDto
        {
            Name = "Updated",
            Email = "updated@test.com",
            UserName = "updated",
            Password = null,
            ThemeId = null
        };

        await _service.UpdateAsync(created.Id, update);

        var user = await _userManager.FindByIdAsync(created.Id.ToString());

        Assert.Equal("updated@test.com", user!.Email);
    }

    // -------------------------
    // Identity Helpers
    // -------------------------

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

    // -------------------------
    // Test Implementations
    // -------------------------

    private class TestUserMapper : IMapper<User, UserRequestDto, UserResponseDto>
    {
        public User ToEntity(UserRequestDto dto)
        {
            return new User
            {
                Name = dto.Name,
                Email = dto.Email,
                UserName = dto.UserName,
                ThemeId = dto.ThemeId
            };
        }

        public void UpdateEntity(UserRequestDto dto, User entity)
        {
            entity.Name = dto.Name;
            entity.Email = dto.Email;
            entity.UserName = dto.UserName;
            entity.ThemeId = dto.ThemeId;
        }

        public UserResponseDto ToDto(User entity)
        {
            return new UserResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                UserName = entity.UserName,
                ThemeId = entity.ThemeId,

                RoleIds = new List<uint>(),
                UserType = "User"
            };
        }
    }

    private class TestUserService(
        ApplicationDbContext dbContext,
        IMapper<User, UserRequestDto, UserResponseDto> mapper,
        UserManager<User> userManager,
        RoleManager<Role> roleManager
    ) : UserService<User, UserRequestDto, UserResponseDto>(
        dbContext,
        mapper,
        userManager,
        roleManager
    )
    {
        protected override DbSet<User> DbSet => DbContext.Set<User>();
    }
}