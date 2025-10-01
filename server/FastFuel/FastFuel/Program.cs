using FastFuel.Models;
using Microsoft.EntityFrameworkCore;

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

        builder.Services.AddGraphQLServer().AddTypes().BindRuntimeType<uint, UnsignedIntType>();

        var app = builder.Build();

        app.MapGraphQL();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.MapNitroApp();
        }

        // ----------- TESTING ONLY -----------
        // This will delete and recreate the database on each run
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            await dbContext.Database.MigrateAsync();

            await SeedDatabaseAsync(dbContext);

            // Get ingredients for big burger
            var bigBurger = dbContext.Foods.Include(f => f.Ingredients).First(f => f.Name == "Big Burger");
            Console.WriteLine($"Ingredients for {bigBurger.Name}:");
            foreach (var ingredient in bigBurger.Ingredients) Console.WriteLine($"- {ingredient.Name}");
        }
        // ----------- END TESTING ONLY -----------

        await app.RunAsync();
    }

    private static async Task SeedDatabaseAsync(ApplicationDbContext context)
    {
        // Check if the database is already seeded
        if (context.Foods.Any()) return; // Database has been seeded

        // Two types of stations: french fries and burgers
        var burgerStation = new StationCategory { Name = "Burger Station" };
        var friesStation = new StationCategory { Name = "Fries Station" };
        context.StationCategories.AddRange(burgerStation, friesStation);
        await context.SaveChangesAsync();

        // Add some allergies
        var glutenAllergy = new Allergy { Name = "Gluten" };
        var dairyAllergy = new Allergy { Name = "Dairy" };
        var peanutAllergy = new Allergy { Name = "Peanuts" };
        context.Allergies.AddRange(glutenAllergy, dairyAllergy, peanutAllergy);
        await context.SaveChangesAsync();

        // Add some ingredients
        var beefPatty = new Ingredient { Name = "Beef Patty", StationCategory = burgerStation };
        var bun = new Ingredient { Name = "Bun", StationCategory = burgerStation, Allergies = [glutenAllergy] };
        var lettuce = new Ingredient { Name = "Lettuce", StationCategory = burgerStation };
        var tomato = new Ingredient { Name = "Tomato", StationCategory = burgerStation };
        var cheese = new Ingredient { Name = "Cheese", StationCategory = burgerStation };
        var potato = new Ingredient { Name = "Potato", StationCategory = friesStation };
        var salt = new Ingredient { Name = "Salt", StationCategory = friesStation };
        var oil = new Ingredient { Name = "Oil", StationCategory = friesStation };
        context.Ingredients.AddRange(beefPatty, bun, lettuce, tomato, cheese, potato, salt, oil);
        await context.SaveChangesAsync();

        // Create some foods
        var bigBurger = new Food
        {
            Name = "Big Burger",
            Price = 800,
            Description = "A big beef burger with lettuce, tomato, and cheese.",
            Ingredients = [beefPatty, bun, lettuce, tomato, cheese]
        };
        var cheeseBurger = new Food
        {
            Name = "Cheese Burger",
            Price = 700,
            Description = "A beef burger with cheese.",
            Ingredients = [beefPatty, bun, cheese]
        };
        var fries = new Food
        {
            Name = "Fries",
            Price = 300,
            Description = "Crispy golden fries.",
            Ingredients = [potato, salt, oil]
        };
        context.Foods.AddRange(bigBurger, cheeseBurger, fries);
        await context.SaveChangesAsync();

        // Create a menu
        var lunchMenu = new Menu
        {
            Name = "Lunch Menu",
            IsSpecialDeal = true,
            Price = 1000,
            Description = "A special lunch menu with a Big Burger and Fries.",
            Foods = [bigBurger, fries]
        };
        context.Menus.Add(lunchMenu);
        await context.SaveChangesAsync();

        // Add a restaurant
        var restaurant = new Restaurant
        {
            Name = "FastFuel Diner",
            Description = "A fast food restaurant serving burgers and fries.",
            Address = "123 Main St, Anytown, USA",
            Latitude = 40.7128,
            Longitude = -74.0060,
            Phone = "555-1234"
        };
        context.Restaurants.Add(restaurant);
        await context.SaveChangesAsync();

        // Add stations to the restaurant
        var burgerStationInstance = new Station
        {
            Name = "Burger Station 1",
            InOperation = true,
            Restaurant = restaurant,
            StationCategory = burgerStation
        };
        var friesStationInstance = new Station
        {
            Name = "Fries Station 1",
            InOperation = true,
            Restaurant = restaurant,
            StationCategory = friesStation
        };
        context.Stations.AddRange(burgerStationInstance, friesStationInstance);
        await context.SaveChangesAsync();

        // Add opening hours
        var openingHours = Enum.GetValues<DayOfWeek>().Select(day => new OpeningHour
        {
            DayOfWeek = day, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(21, 0), Restaurant = restaurant
        }).ToList();
        context.OpeningHours.AddRange(openingHours);
        await context.SaveChangesAsync();

        // Place an order
        var order = new Order
        {
            Restaurant = restaurant,
            OrderNumber = 1,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        // TODO: save price at order time
        // This is important because menu and food prices may change over time

        // Add a menu and an extra food item to the order
        var orderMenuItem = new OrderMenu
        {
            Menu = lunchMenu,
            Order = order,
            Quantity = 1
        };

        var orderFoodItem = new OrderFood
        {
            Food = cheeseBurger,
            Order = order,
            Quantity = 1
        };
        context.OrderMenus.Add(orderMenuItem);
        context.OrderFoods.Add(orderFoodItem);
        await context.SaveChangesAsync();
    }
}