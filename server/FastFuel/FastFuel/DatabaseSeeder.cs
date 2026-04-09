using FastFuel.Features.Admins.DTOs;
using FastFuel.Features.Admins.Entities;
using FastFuel.Features.Allergies.Entities;
using FastFuel.Features.Common.DbContexts;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Customers.Entities;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Employees.Entities;
using FastFuel.Features.FoodIngredients.Entities;
using FastFuel.Features.Foods.Entities;
using FastFuel.Features.Ingredients.Entities;
using FastFuel.Features.Machines.DTOs;
using FastFuel.Features.Machines.Entities;
using FastFuel.Features.MenuFoods.Entities;
using FastFuel.Features.Menus.Entities;
using FastFuel.Features.OpeningHours.Entities;
using FastFuel.Features.OrderFoods.Entities;
using FastFuel.Features.OrderMenus.Entities;
using FastFuel.Features.Orders.Common;
using FastFuel.Features.Orders.Entities;
using FastFuel.Features.Restaurants.Entities;
using FastFuel.Features.StationCategories.Entities;
using FastFuel.Features.Stations.Entities;
using FastFuel.Features.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastFuel;

// TODO: Rewrite this to use the services instead of directly accessing the DbContext.
public class DatabaseSeeder(IServiceProvider serviceProvider)
{
    private readonly ICrudService<AdminRequestDto, AdminResponseDto> _adminService =
        serviceProvider.GetRequiredService<ICrudService<AdminRequestDto, AdminResponseDto>>();

    private readonly FastFuelDbContext _context = serviceProvider.GetRequiredService<FastFuelDbContext>();

    private readonly ICrudService<CustomerRequestDto, CustomerResponseDto> _customerService =
        serviceProvider.GetRequiredService<ICrudService<CustomerRequestDto, CustomerResponseDto>>();

    private readonly ICrudService<EmployeeRequestDto, EmployeeResponseDto> _employeeService =
        serviceProvider.GetRequiredService<ICrudService<EmployeeRequestDto, EmployeeResponseDto>>();

    private readonly ICrudService<MachineRequestDto, MachineResponseDto> _machineService =
        serviceProvider.GetRequiredService<ICrudService<MachineRequestDto, MachineResponseDto>>();

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

        await SeedAdmin();
        await SeedEmployee(restaurant);
        await SeedMachine(restaurant);
        var customer = await SeedCustomer();

        // Place an order
        var order = new Order
        {
            Restaurant = restaurant,
            OrderNumber = 1,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            User = customer
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Add a menu and an extra food item to the order
        var orderMenuItem = new OrderMenu
        {
            Menu = lunchMenu,
            Order = order,
            Quantity = 1,
            SpecialInstructions = "Please make the burger without tomato."
        };

        var orderFoodItem = new OrderFood
        {
            Food = cheeseBurger,
            Order = order,
            Quantity = 1,
            SpecialInstructions = "Extra cheese, please."
        };
        _context.OrderMenus.Add(orderMenuItem);
        _context.OrderFoods.Add(orderFoodItem);
        await _context.SaveChangesAsync();
    }

    public async Task SeedProdAsync()
    {
        if (await _context.Users.AnyAsync()) return;

        await SeedAdmin();
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private async Task<Employee> SeedEmployee(Restaurant workplace)
    {
        var userName = "employee";

        var employeeDto = new EmployeeRequestDto
        {
            UserName = userName,
            Email = "employee@example.com",
            Name = "Employee User",
            Password = "Employee123!",
            ShiftIds = [],
            StationCategoryIds = [],
            WorksAtRestaurantId = workplace.Id
        };

        await _employeeService.CreateAsync(employeeDto);
        var employee = await _userManager.FindByNameAsync(userName);
        return Task.FromResult(employee as Employee).Result!;
    }

    private async Task<Customer> SeedCustomer()
    {
        var userName = "customer";

        var customerDto = new CustomerRequestDto
        {
            UserName = userName,
            Email = "customer@example.com",
            Name = "Customer User",
            Password = "Customer123!"
        };

        await _customerService.CreateAsync(customerDto);

        var customer = await _userManager.FindByNameAsync(userName);
        return Task.FromResult(customer as Customer).Result!;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private async Task<Admin> SeedAdmin()
    {
        var userName = "admin";

        var adminRequestDto = new AdminRequestDto
        {
            UserName = userName,
            Email = "admin@example.com",
            Name = "Admin User",
            Password = "Admin123!"
        };

        await _adminService.CreateAsync(adminRequestDto);

        var adminUser = await _userManager.FindByNameAsync(userName);
        return Task.FromResult(adminUser as Admin).Result!;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private async Task<Machine> SeedMachine(Restaurant locatedAt)
    {
        var userName = "machine";

        var machineRequestDto = new MachineRequestDto
        {
            UserName = userName,
            LocatedAtRestaurantId = locatedAt.Id,
            Name = "Machine User",
            Password = "Machine123!"
        };

        await _machineService.CreateAsync(machineRequestDto);

        var machineUser = await _userManager.FindByNameAsync(userName);
        return Task.FromResult(machineUser as Machine).Result!;
    }
}