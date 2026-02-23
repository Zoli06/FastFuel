using EntityFramework.Exceptions.MySQL.Pomelo;
using FastFuel.Features.Allergies.Entities;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.FoodIngredients.Entities;
using FastFuel.Features.Foods.Entities;
using FastFuel.Features.Ingredients.Entities;
using FastFuel.Features.MenuFoods.Entities;
using FastFuel.Features.Menus.Entities;
using FastFuel.Features.OpeningHours.Entities;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.Shifts.Entities;
using FastFuel.Features.StationCategories.Entities;
using FastFuel.Features.Stations.Entities;
using FastFuel.Features.Themes.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.DbContexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<User, Role, uint>(options)
{
    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
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
    public DbSet<StationCategory> StationCategories { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Theme> Themes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Entity<User>().UseTptMappingStrategy();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseExceptionProcessor();
    }
}