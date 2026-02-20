using FastFuel.Features.Auth;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.ExceptionFilters;
using FastFuel.Features.Users.Models;
using FastFuel.NSwag;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        // ----------- TESTING ONLY -----------
        // This will delete and recreate the database on each run
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
            await DatabaseSeeder.SeedAsync(dbContext, passwordHasher);
        }
        // ----------- END TESTING ONLY -----------

        await app.RunAsync();
    }

    // Configures JWT authentication and IdentityCore for User management
    private static void ConfigureAuth(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie();

        builder.Services.AddAuthorization();
        builder.Services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();
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
            config.OperationProcessors.Add(new UnauthorizedHttpResultOperationProcessor());
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
            .AddClasses(filter => filter.InNamespaces("FastFuel.Features"))
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
        app.MapGroup("/api/Auth").WithTags("Auth").MapCustomIdentityApi<User>();

        app.MapControllers();
    }
}