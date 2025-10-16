using FastFuel.Models.IngredientAllergies;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Models;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Allergy> Allergies { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<FoodIngredient> FoodIngredients { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<IngredientAllergy> IngredientAllergies { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuFood> MenuFoods { get; set; }
    public DbSet<OpeningHour> OpeningHours { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderFood> OrderFoods { get; set; }
    public DbSet<OrderMenu> OrderMenus { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<StationCategory> StationCategories { get; set; }
    public DbSet<StationCategoryIngredient> StationCategoryIngredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ingredient>()
            .HasMany(f => f.Allergies)
            .WithMany(i => i.Ingredients)
            .UsingEntity<IngredientAllergy>();
        
        modelBuilder.Entity<Food>()
            .HasMany(f => f.Ingredients)
            .WithMany(i => i.Foods)
            .UsingEntity<FoodIngredient>();

        modelBuilder.Entity<Menu>()
            .HasMany(m => m.Foods)
            .WithMany(f => f.Menus)
            .UsingEntity<MenuFood>();

        modelBuilder.Entity<Ingredient>()
            .HasMany(i => i.StationCategories)
            .WithMany(sc => sc.Ingredients)
            .UsingEntity<StationCategoryIngredient>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}