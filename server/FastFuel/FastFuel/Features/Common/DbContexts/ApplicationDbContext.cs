using EntityFramework.Exceptions.MySQL.Pomelo;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Customers.Models;
using FastFuel.Features.Employees.Models;
using FastFuel.Features.FoodIngredients.Models;
using FastFuel.Features.Foods.Models;
using FastFuel.Features.Ingredients.Models;
using FastFuel.Features.MenuFoods.Models;
using FastFuel.Features.Menus.Models;
using FastFuel.Features.OpeningHours.Models;
using FastFuel.Features.OrderFoods.Models;
using FastFuel.Features.OrderMenus.Models;
using FastFuel.Features.Orders.Models;
using FastFuel.Features.Restaurants.Models;
using FastFuel.Features.StationCategories.Models;
using FastFuel.Features.Stations.Models;
using FastFuel.Features.Themes.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<FoodIngredient> FoodIngredients { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<MenuFood> MenuFoods { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<OpeningHour> OpeningHours { get; set; }
    public DbSet<OrderFood> OrderFoods { get; set; }
    public DbSet<OrderMenu> OrderMenus { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<StationCategory> StationCategories { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Theme> Themes { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }
}