using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.ExceptionFilters;
using FastFuel.Features.Roles.Models;
using FastFuel.Features.Users.Models;
using FastFuel.NSwag;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using Scrutor;

namespace FastFuel;

public static class Program
{
    public static void Main(string[] args)
    {
        MainAsync(args).GetAwaiter().GetResult();
    }

    private static async Task MainAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure authorization and authentication (JWT)
        ConfigureAuth(builder);

        // Configure database
        ConfigureDatabase(builder);

        // Configure other application services (controllers, OpenAPI, CORS, DI)
        ConfigureAppServices(builder);

        var app = builder.Build();

        // Configure middleware / request pipeline
        ConfigureMiddleware(app);

        // Seed the database with initial data (roles, users, etc.)
        await SeedDatabaseAsync(app);

        await app.RunAsync();
    }

    // Configures JWT authentication and IdentityCore for User management
    private static void ConfigureAuth(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
        builder.Services
            .AddIdentityApiEndpoints<User>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    // Configures the application's EF Core DbContext
    private static void ConfigureDatabase(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(dbContextOptions =>
        {
            dbContextOptions
                .UseLazyLoadingProxies()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            if (builder.Environment.IsDevelopment())
                dbContextOptions.LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
        });
    }

    // Registers controllers, OpenAPI, CORS, password hasher and scans feature services
    private static void ConfigureAppServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            // Convert UniqueConstraintException from EF into HTTP 409 responses globally
            options.Filters.Add<UniqueConstraintExceptionFilter>();
            // TODO: Add filter for reference constraint exceptions
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument(config =>
        {
            config.OperationProcessors.Add(new UnregisteredStatusCodeResultOperationProcessor());

            config.AddSecurity("Bearer", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                In = OpenApiSecurityApiKeyLocation.Header,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.Scan(scan => scan
            .FromAssemblies(typeof(Program).Assembly)
            .AddClasses(filter => filter
                .InNamespaces("FastFuel.Features")
                .Where(t => !typeof(IFilterMetadata).IsAssignableFrom(t)
                            && !typeof(IFilterFactory).IsAssignableFrom(t)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    // Configure middleware pipeline (CORS, Dev tools, Authentication, Authorization, Controllers)
    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseCors("AllowAll");
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseOpenApi();
            app.UseSwaggerUi();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
    }

    // Seed the database with initial data (roles, users, etc.) on application startup
    private static async Task SeedDatabaseAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var databaseSeeder = new DatabaseSeeder(scope.ServiceProvider);
        if (app.Environment.IsDevelopment())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            await databaseSeeder.SeedTestAsync();
        }
        else
        {
            await databaseSeeder.SeedAsync();
        }
    }
}