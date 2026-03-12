using System.Text.Json.Serialization;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Exceptions;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Roles.Services;
using FastFuel.Features.Users.Entities;
using FastFuel.NSwag.SwaggerQueryParam;
using FastFuel.NSwag.UnregisteredStatusCodeResultOperation;
using Microsoft.AspNetCore.Diagnostics;
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

        // Configure global exception handlers
        AddExceptionHandler(builder);

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

            if (!builder.Environment.IsDevelopment())
                return;

            dbContextOptions
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        });
    }

    // Registers controllers, OpenAPI, CORS, password hasher and scans feature services
    private static void ConfigureAppServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument(config =>
        {
            config.Title = "FastFuel";
            config.OperationProcessors.Add(new UnregisteredStatusCodeResultOperationProcessor());
            config.OperationProcessors.Add(new SwaggerQueryParamProcessor());

            config.AddSecurity("Bearer", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                In = OpenApiSecurityApiKeyLocation.Header,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });

        builder.Services.AddCors();

        builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddScoped<IDefaultRoleInitializer, DefaultRoleInitializer>();
        builder.Services.Scan(scan => scan
            .FromAssemblies(typeof(Program).Assembly)
            .AddClasses(filter => filter
                .InNamespaces("FastFuel.Features")
                // TODO: switch to an opt-in approach
                .Where(t => !typeof(IFilterMetadata).IsAssignableFrom(t)
                            && !typeof(IFilterFactory).IsAssignableFrom(t)
                            && !typeof(IExceptionHandler).IsAssignableFrom(t)
                            && !typeof(Exception).IsAssignableFrom(t)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    private static void AddExceptionHandler(WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    }

    // Configure middleware pipeline (CORS, Dev tools, Authentication, Authorization, Controllers)
    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseCors(options =>
        {
            // TODO: Remove this security issue
            options.SetIsOriginAllowed(_ => true);
            options.AllowAnyMethod();
            options.AllowAnyHeader();
            options.AllowCredentials();
        });

        if (app.Environment.IsDevelopment())
        {
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

        if (app.Environment.IsDevelopment())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
        }

        var roleInitializer = scope.ServiceProvider.GetRequiredService<IDefaultRoleInitializer>();
        await roleInitializer.InitializeAsync();

        var databaseSeeder = new DatabaseSeeder(scope.ServiceProvider);
        if (app.Environment.IsDevelopment())
            await databaseSeeder.SeedTestAsync();
        else
            await databaseSeeder.SeedAsync();
    }
}