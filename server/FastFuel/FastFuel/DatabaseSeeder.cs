using System.Security.Claims;
using FastFuel.Features.Allergies.Entities;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.FoodIngredients.Entities;
using FastFuel.Features.Foods.Entities;
using FastFuel.Features.Ingredients.Entities;
using FastFuel.Features.MenuFoods.Entities;
using FastFuel.Features.Menus.Entities;
using FastFuel.Features.OpeningHours.Entities;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.Permissions.Services;
using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.Roles.Entities;
using FastFuel.Features.StationCategories.Entities;
using FastFuel.Features.Stations.Entities;
using FastFuel.Features.Themes.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel;

// TODO: Rewrite this to use the services instead of directly accessing the DbContext.
public class DatabaseSeeder(IServiceProvider serviceProvider)
{
    private readonly ApplicationDbContext _context = serviceProvider.GetRequiredService<ApplicationDbContext>();

    private readonly ICrudService<CustomerRequestDto, CustomerResponseDto> _customerService =
        serviceProvider.GetRequiredService<ICrudService<CustomerRequestDto, CustomerResponseDto>>();

    private readonly ICrudService<EmployeeRequestDto, EmployeeResponseDto> _employeeService =
        serviceProvider.GetRequiredService<ICrudService<EmployeeRequestDto, EmployeeResponseDto>>();

    private readonly IPermissionService _permissionService = serviceProvider.GetRequiredService<IPermissionService>();
    private readonly RoleManager<Role> _roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
    private readonly UserManager<User> _userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    public async Task SeedTestAsync()
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
            CreatedAt = DateTime.UtcNow,
            Customer = await _context.Users.OfType<Customer>().FirstOrDefaultAsync(c => c.UserName == "customer")
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

    public async Task SeedAsync()
    {
        if (await _context.Users.AnyAsync()) return;

        await SeedAdmin();
    }

    private async Task SeedEmployee()
    {
        var employeeDto = new EmployeeRequestDto
        {
            UserName = "employee",
            Email = "employee@example.com",
            Name = "Employee User",
            Password = "Employee123!",
            ThemeId = null,
            ShiftIds = [],
            StationCategoryIds = []
        };

        await _employeeService.CreateAsync(employeeDto);
    }

    private async Task SeedCustomer()
    {
        var customerDto = new CustomerRequestDto
        {
            UserName = "customer",
            Email = "customer@example.com",
            Name = "Customer User",
            Password = "Customer123!",
            ThemeId = null
        };

        await _customerService.CreateAsync(customerDto);
    }

    private async Task SeedAdmin()
    {
        var adminDto = new EmployeeRequestDto
        {
            UserName = "admin",
            Email = "admin@example.com",
            Name = "Admin User",
            Password = "Admin123!",
            ThemeId = null,
            ShiftIds = [],
            StationCategoryIds = []
        };

        await _employeeService.CreateAsync(adminDto);

        var adminUser = await _userManager.FindByNameAsync("admin");
        var allPermissions = _permissionService.GetAllPermissions();
        var role = await _roleManager.FindByNameAsync("Admin");
        if (role == null)
        {
            role = new Role { Name = "Admin" };
            await _roleManager.CreateAsync(role);
            foreach (var permission in allPermissions)
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }

        await _userManager.AddToRoleAsync(adminUser!, "Admin");
    }
}