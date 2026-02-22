using System.Security.Claims;
using FastFuel.Features.Allergies.Models;
using FastFuel.Features.Common.DbContexts;
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
using FastFuel.Features.Permissions.Services;
using FastFuel.Features.Restaurants.Models;
using FastFuel.Features.Roles.Models;
using FastFuel.Features.StationCategories.Models;
using FastFuel.Features.Stations.Models;
using FastFuel.Features.Themes.Models;
using FastFuel.Features.Users.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel;

public class DatabaseSeeder(IServiceProvider serviceProvider)
{
    private readonly ApplicationDbContext _context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    private readonly IPermissionService _permissionService = serviceProvider.GetRequiredService<IPermissionService>();
    private readonly RoleManager<Role> _roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
    private readonly UserManager<User> _userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    // Get services from the DI container

    public async Task SeedAsync()
    {
        // Two types of stations: french fries and burgers
        var burgerStation = new StationCategory { Name = "Burger Station" };
        var friesStation = new StationCategory { Name = "Fries Station" };
        _context.StationCategories.AddRange(burgerStation, friesStation);
        await _context.SaveChangesAsync();

        // Add some allergies
        var glutenAllergy = new Allergy { Name = "Gluten" };
        var dairyAllergy = new Allergy { Name = "Dairy" };
        var peanutAllergy = new Allergy { Name = "Peanuts" };
        _context.Allergies.AddRange(glutenAllergy, dairyAllergy, peanutAllergy);
        await _context.SaveChangesAsync();

        // Add some ingredients
        var beefPatty = new Ingredient { Name = "Beef Patty" };
        var bun = new Ingredient { Name = "Bun", Allergies = [glutenAllergy] };
        var lettuce = new Ingredient { Name = "Lettuce" };
        var tomato = new Ingredient { Name = "Tomato" };
        var cheese = new Ingredient { Name = "Cheese" };
        var potato = new Ingredient { Name = "Potato" };
        var salt = new Ingredient { Name = "Salt" };
        var oil = new Ingredient { Name = "Oil" };
        _context.Ingredients.AddRange(beefPatty, bun, lettuce, tomato, cheese, potato, salt, oil);
        await _context.SaveChangesAsync();

        // Assign ingredients to station categories
        burgerStation.Ingredients.AddRange([beefPatty, bun, lettuce, tomato, cheese]);
        friesStation.Ingredients.AddRange([potato, salt, oil]);
        await _context.SaveChangesAsync();

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
        _context.Foods.AddRange(bigBurger, cheeseBurger, fries);
        await _context.SaveChangesAsync();

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
        _context.Menus.Add(lunchMenu);
        await _context.SaveChangesAsync();

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
        _context.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

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
        _context.Stations.AddRange(burgerStationInstance, friesStationInstance);
        await _context.SaveChangesAsync();

        // Add opening hours
        var openingHours = Enum.GetValues<DayOfWeek>().Select(day => new OpeningHour
        {
            DayOfWeek = day, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(21, 0), Restaurant = restaurant
        }).ToList();
        _context.OpeningHours.AddRange(openingHours);
        await _context.SaveChangesAsync();

        // Register user with a theme
        var theme = new Theme
        {
            Name = "Dark Mode",
            Background = "#121212",
            Footer = "#1e1e1e",
            ButtonPrimary = "#bb86fc",
            ButtonSecondary = "#03dac6"
        };

        _context.Themes.Add(theme);
        await _context.SaveChangesAsync();

        await SeedAdmin();
        await SeedEmployee();
        await SeedCustomer();

        // Place an order
        var order = new Order
        {
            Restaurant = restaurant,
            OrderNumber = 1,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

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
        _context.OrderMenus.Add(orderMenuItem);
        _context.OrderFoods.Add(orderFoodItem);
        await _context.SaveChangesAsync();
    }

    private async Task SeedEmployee()
    {
        var employee = new Employee
        {
            UserName = "employee1",
            Email = "employee@example.com",
            Name = "Employee One"
        };

        var permissions = new[]
        {
            "Permission:Order:Create",
            "Permission:Order:Read",
            "Permission:Order:Update",
            "Permission:Order:Delete",
            "Permission:Station:Read",
            "Permission:StationCategory:Read",
            "Permission:Menu:Read",
            "Permission:Food:Read",
            "Permission:Ingredient:Read",
            "Permission:Allergy:Read",
            "Permission:Customer:Read"
        };

        var result = await _userManager.CreateAsync(employee, "Employee123!");
        if (!result.Succeeded) return;

        var role = await _roleManager.FindByNameAsync("Employee");
        if (role == null)
        {
            role = new Role { Name = "Employee" };
            await _roleManager.CreateAsync(role);
            foreach (var permission in permissions)
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }

        await _userManager.AddToRoleAsync(employee, "Employee");
    }

    private async Task SeedCustomer()
    {
        var customer = new Customer
        {
            UserName = "customer1",
            Email = "customer@example.com",
            Name = "Customer One",
            Theme = await _context.Themes.FirstOrDefaultAsync()
        };

        var permissions = new[]
        {
            "Permission:Order:Create",
            "Permission:Menu:Read",
            "Permission:Food:Read",
            "Permission:Ingredient:Read",
            "Permission:Allergy:Read",
            "Permission:Restaurant:Read"
        };

        var result = await _userManager.CreateAsync(customer, "Customer123!");
        if (!result.Succeeded) return;

        var role = await _roleManager.FindByNameAsync("Customer");
        if (role == null)
        {
            role = new Role { Name = "Customer" };
            await _roleManager.CreateAsync(role);
            foreach (var permission in permissions)
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }

        await _userManager.AddToRoleAsync(customer, "Customer");
    }

    private async Task SeedAdmin()
    {
        var admin = new Employee
        {
            UserName = "admin",
            Email = "admin@example.com",
            Name = "Admin User"
        };

        // Get all permissions from the PermissionService
        var allPermissions = _permissionService.GetAllPermissions();

        var result = await _userManager.CreateAsync(admin, "Admin123!");
        if (!result.Succeeded) return;

        var role = await _roleManager.FindByNameAsync("Admin");
        if (role == null)
        {
            role = new Role { Name = "Admin" };
            await _roleManager.CreateAsync(role);
            foreach (var permission in allPermissions)
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }

        await _userManager.AddToRoleAsync(admin, "Admin");
    }
}