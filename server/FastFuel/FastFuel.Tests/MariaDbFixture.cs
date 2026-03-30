using FastFuel.Features.Common.DbContexts;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MariaDb;

namespace FastFuel.Tests;

public class MariaDbFixture : IAsyncLifetime
{
    private readonly MariaDbContainer _mariaDbContainer = new MariaDbBuilder("mariadb:latest")
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpassword")
        .Build();

    private string ConnectionString => _mariaDbContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _mariaDbContainer.StartAsync();
        await using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _mariaDbContainer.DisposeAsync();
    }

    public ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseLazyLoadingProxies()
            .UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString))
            .Options;

        return new ApplicationDbContext(options);
    }
}