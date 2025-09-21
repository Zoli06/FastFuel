using FastFuel.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseMySQL(connectionString); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

// Place your endpoints here

// ----------- TESTING ONLY -----------
// This will delete and recreate the database on each run
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();
dbContext.Database.Migrate();

dbContext.Foods.Add(new Food
{
    Name = "Test Food",
    Price = 999,
    Description = "This is a test food item."
});
dbContext.SaveChanges();
// ----------- END TESTING ONLY -----------

app.Run();