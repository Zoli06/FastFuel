using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common;
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

        builder.Services.AddControllers();

        builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Program).Assembly));
        if (builder.Environment.IsDevelopment())
            builder.Services.AddEndpointsApiExplorer();

        // Here lies the GraphQL server setup—once powering queries, now retired for simpler times.
        // builder.Services.AddGraphQLServer().AddQueryType<Query>().AddTypes().BindRuntimeType<uint, UnsignedIntType>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // app.UseHttpsRedirection();
        app.MapControllers();

        app.UseCors("AllowAll");
        if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

        // ----------- TESTING ONLY -----------
        // This will delete and recreate the database on each run
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            await dbContext.Database.MigrateAsync();

            await SeedDatabaseAsync(dbContext);
        }
        // ----------- END TESTING ONLY -----------

        await app.RunAsync();
    }

    private static async Task SeedDatabaseAsync(ApplicationDbContext context)
    {
        // Check if the database is already seeded
        // if (context.Foods.Any()) return; // Database has been seeded

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
        var beefPatty = new Ingredient { Name = "Beef Patty" };
        var bun = new Ingredient { Name = "Bun", Allergies = [glutenAllergy] };
        var lettuce = new Ingredient { Name = "Lettuce" };
        var tomato = new Ingredient { Name = "Tomato" };
        var cheese = new Ingredient { Name = "Cheese" };
        var potato = new Ingredient { Name = "Potato" };
        var salt = new Ingredient { Name = "Salt" };
        var oil = new Ingredient { Name = "Oil" };
        context.Ingredients.AddRange(beefPatty, bun, lettuce, tomato, cheese, potato, salt, oil);
        await context.SaveChangesAsync();

        // Assign ingredients to station categories
        burgerStation.Ingredients = [beefPatty, bun, lettuce, tomato];
        friesStation.Ingredients = [potato, salt, oil];
        await context.SaveChangesAsync();

        // Create some foods
        var bigBurger = new Food
        {
            Name = "Big Burger",
            Price = 800,
            Description = "A big beef burger with lettuce, tomato, and cheese.",
            FoodIngredients =
            [
                new FoodIngredient { Ingredient = beefPatty, Quantity = 1 },
                new FoodIngredient { Ingredient = bun, Quantity = 1 },
                new FoodIngredient { Ingredient = lettuce, Quantity = 2 },
                new FoodIngredient { Ingredient = tomato, Quantity = 2 },
                new FoodIngredient { Ingredient = cheese, Quantity = 1 }
            ],
            ImageUrl = new Uri("https://cdn.pixabay.com/photo/2022/08/29/17/44/burger-7419420_1280.jpg")
        };
        var cheeseBurger = new Food
        {
            Name = "Cheese Burger",
            Price = 700,
            Description = "A beef burger with cheese.",
            FoodIngredients =
            [
                new FoodIngredient { Ingredient = beefPatty, Quantity = 1 },
                new FoodIngredient { Ingredient = bun, Quantity = 1 },
                new FoodIngredient { Ingredient = cheese, Quantity = 1 }
            ]
        };
        var fries = new Food
        {
            Name = "Fries",
            Price = 300,
            Description = "Crispy golden fries.",
            FoodIngredients =
            [
                new FoodIngredient { Ingredient = potato, Quantity = 3 },
                new FoodIngredient { Ingredient = salt, Quantity = 1 },
                new FoodIngredient { Ingredient = oil, Quantity = 1 }
            ]
        };
        context.Foods.AddRange(bigBurger, cheeseBurger, fries);
        await context.SaveChangesAsync();

        // Create a menu (use MenuFood join entities)
        var lunchMenu = new Menu
        {
            Name = "Lunch Menu",
            Price = 1000,
            Description = "A special lunch menu with a Big Burger and Fries.",
            MenuFoods =
            [
                new MenuFood { Food = bigBurger, Quantity = 1 },
                new MenuFood { Food = fries, Quantity = 1 }
            ]
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