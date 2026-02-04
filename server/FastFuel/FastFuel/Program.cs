using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FastFuel.Features.Authentication.Settings;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Restaurants.Models;
using FastFuel.NSwag;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        // Configure JWT and authentication, throws if missing
        ConfigureJwtAndAuthentication(builder);

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

            await DatabaseSeeder.SeedAsync(dbContext);
        }
        // ----------- END TESTING ONLY -----------

        await app.RunAsync();
    }

    // Binds JwtSettings, registers authentication and the IJwtSettings singleton
    private static void ConfigureJwtAndAuthentication(WebApplicationBuilder builder)
    {
        var jwtSection = builder.Configuration.GetSection("JwtSettings");
        builder.Services.Configure<JwtSettings>(jwtSection);
        var jwtSettings = jwtSection.Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JWT settings are not configured properly.");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "JwtBearer";
            options.DefaultChallengeScheme = "JwtBearer";
        }).AddJwtBearer("JwtBearer", options =>
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = signingKey,
                ClockSkew = TimeSpan.Zero
            };
        });
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
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
        builder.Services.AddControllers();

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

        builder.Services.AddTransient<IPasswordHasher<Restaurant>, PasswordHasher<Restaurant>>();

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

        app.MapControllers();
    }
}