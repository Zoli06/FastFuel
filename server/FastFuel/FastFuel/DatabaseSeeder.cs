using FastFuel.Features.Admins.DTOs;
using FastFuel.Features.Allergies.DTOs;
using FastFuel.Features.Common.Services;
using FastFuel.Features.Customers.DTOs;
using FastFuel.Features.Employees.DTOs;
using FastFuel.Features.Foods.DTOs;
using FastFuel.Features.Ingredients.DTOs;
using FastFuel.Features.Machines.DTOs;
using FastFuel.Features.Menus.DTOs;
using FastFuel.Features.Orders.DTOs;
using FastFuel.Features.Orders.Services;
using FastFuel.Features.Restaurants.DTOs;
using FastFuel.Features.StationCategories.DTOs;
using FastFuel.Features.Stations.DTOs;
using FastFuel.Features.Stations.Services;
using FastFuel.Features.Users.DTOs;

namespace FastFuel;

public class DatabaseSeeder(IServiceProvider serviceProvider)
{
    private readonly ICrudService<AdminRequestDto, AdminResponseDto> _adminService =
        serviceProvider.GetRequiredService<ICrudService<AdminRequestDto, AdminResponseDto>>();

    private readonly ICrudService<AllergyRequestDto, AllergyResponseDto> _allergyService =
        serviceProvider.GetRequiredService<ICrudService<AllergyRequestDto, AllergyResponseDto>>();

    private readonly ICrudService<FoodRequestDto, FoodResponseDto> _foodService =
        serviceProvider.GetRequiredService<ICrudService<FoodRequestDto, FoodResponseDto>>();

    private readonly ICrudService<IngredientRequestDto, IngredientResponseDto> _ingredientService =
        serviceProvider.GetRequiredService<ICrudService<IngredientRequestDto, IngredientResponseDto>>();

    private readonly ICrudService<CustomerRequestDto, CustomerResponseDto> _customerService =
        serviceProvider.GetRequiredService<ICrudService<CustomerRequestDto, CustomerResponseDto>>();

    private readonly ICrudService<EmployeeRequestDto, EmployeeResponseDto> _employeeService =
        serviceProvider.GetRequiredService<ICrudService<EmployeeRequestDto, EmployeeResponseDto>>();

    private readonly ICrudService<MachineRequestDto, MachineResponseDto> _machineService =
        serviceProvider.GetRequiredService<ICrudService<MachineRequestDto, MachineResponseDto>>();

    private readonly ICrudService<MenuRequestDto, MenuResponseDto> _menuService =
        serviceProvider.GetRequiredService<ICrudService<MenuRequestDto, MenuResponseDto>>();

    private readonly IOrderService _orderService = serviceProvider.GetRequiredService<IOrderService>();

    private readonly ICrudService<RestaurantRequestDto, RestaurantResponseDto> _restaurantService =
        serviceProvider.GetRequiredService<ICrudService<RestaurantRequestDto, RestaurantResponseDto>>();

    private readonly ICrudService<StationCategoryRequestDto, StationCategoryResponseDto> _stationCategoryService =
        serviceProvider.GetRequiredService<ICrudService<StationCategoryRequestDto, StationCategoryResponseDto>>();

    private readonly IStationService _stationService = serviceProvider.GetRequiredService<IStationService>();

    private readonly ICrudService<UserRequestDto, UserResponseDto> _userService =
        serviceProvider.GetRequiredService<ICrudService<UserRequestDto, UserResponseDto>>();

    public async Task SeedTestAsync()
    {
        var burgerStation = await _stationCategoryService.CreateAsync(new StationCategoryRequestDto
        {
            Name = "Burger Station",
            IngredientIds = []
        });
        var friesStation = await _stationCategoryService.CreateAsync(new StationCategoryRequestDto
        {
            Name = "Fries Station",
            IngredientIds = []
        });

        var glutenAllergy = await _allergyService.CreateAsync(new AllergyRequestDto
        {
            Name = "Gluten",
            Message = "Contains gluten.",
            IngredientIds = []
        });
        var dairyAllergy = await _allergyService.CreateAsync(new AllergyRequestDto
        {
            Name = "Dairy",
            Message = "Contains dairy.",
            IngredientIds = []
        });
        await _allergyService.CreateAsync(new AllergyRequestDto
        {
            Name = "Peanuts",
            Message = "Contains peanuts.",
            IngredientIds = []
        });

        var beefPatty = await _ingredientService.CreateAsync(new IngredientRequestDto
        {
            Name = "Beef Patty",
            AllergyIds = [],
            StationCategoryIds = [burgerStation.Id]
        });
        var bun = await _ingredientService.CreateAsync(new IngredientRequestDto
        {
            Name = "Bun",
            AllergyIds = [glutenAllergy.Id],
            StationCategoryIds = [burgerStation.Id]
        });
        var lettuce = await _ingredientService.CreateAsync(new IngredientRequestDto
        {
            Name = "Lettuce",
            AllergyIds = [],
            StationCategoryIds = [burgerStation.Id]
        });
        var tomato = await _ingredientService.CreateAsync(new IngredientRequestDto
        {
            Name = "Tomato",
            AllergyIds = [],
            StationCategoryIds = [burgerStation.Id]
        });
        var cheese = await _ingredientService.CreateAsync(new IngredientRequestDto
        {
            Name = "Cheese",
            AllergyIds = [dairyAllergy.Id],
            StationCategoryIds = [burgerStation.Id]
        });
        var potato = await _ingredientService.CreateAsync(new IngredientRequestDto
        {
            Name = "Potato",
            AllergyIds = [],
            StationCategoryIds = [friesStation.Id]
        });
        var salt = await _ingredientService.CreateAsync(new IngredientRequestDto
        {
            Name = "Salt",
            AllergyIds = [],
            StationCategoryIds = [friesStation.Id]
        });
        var oil = await _ingredientService.CreateAsync(new IngredientRequestDto
        {
            Name = "Oil",
            AllergyIds = [],
            StationCategoryIds = [friesStation.Id]
        });

        var bigBurger = await _foodService.CreateAsync(new FoodRequestDto
        {
            Name = "Big Burger",
            Price = 800,
            Description = "A big beef burger with lettuce, tomato, and cheese.",
            ImageUrl = new Uri("https://cdn.pixabay.com/photo/2022/08/29/17/44/burger-7419420_1280.jpg"),
            Ingredients =
            [
                new FoodIngredientDto { IngredientId = beefPatty.Id, Quantity = 1, Unit = "piece" },
                new FoodIngredientDto { IngredientId = bun.Id, Quantity = 1, Unit = "piece" },
                new FoodIngredientDto { IngredientId = lettuce.Id, Quantity = 2, Unit = "leaf" },
                new FoodIngredientDto { IngredientId = tomato.Id, Quantity = 2, Unit = "slice" },
                new FoodIngredientDto { IngredientId = cheese.Id, Quantity = 1, Unit = "slice" }
            ]
        });
        var cheeseBurger = await _foodService.CreateAsync(new FoodRequestDto
        {
            Name = "Cheese Burger",
            Price = 700,
            Description = "A beef burger with cheese.",
            ImageUrl = null,
            Ingredients =
            [
                new FoodIngredientDto { IngredientId = beefPatty.Id, Quantity = 1, Unit = "piece" },
                new FoodIngredientDto { IngredientId = bun.Id, Quantity = 1, Unit = "piece" },
                new FoodIngredientDto { IngredientId = cheese.Id, Quantity = 1, Unit = "slice" }
            ]
        });
        var fries = await _foodService.CreateAsync(new FoodRequestDto
        {
            Name = "Fries",
            Price = 300,
            Description = "Crispy golden fries.",
            ImageUrl = null,
            Ingredients =
            [
                new FoodIngredientDto { IngredientId = potato.Id, Quantity = 3, Unit = "piece" },
                new FoodIngredientDto { IngredientId = salt.Id, Quantity = 1, Unit = "pinch" },
                new FoodIngredientDto { IngredientId = oil.Id, Quantity = 1, Unit = "portion" }
            ]
        });

        var lunchMenu = await _menuService.CreateAsync(new MenuRequestDto
        {
            Name = "Lunch Menu",
            Price = 1000,
            Description = "A special lunch menu with a Big Burger and Fries.",
            ImageUrl = null,
            Foods =
            [
                new MenuFoodDto { FoodId = bigBurger.Id, Quantity = 1 },
                new MenuFoodDto { FoodId = fries.Id, Quantity = 1 }
            ]
        });

        var restaurant = await _restaurantService.CreateAsync(new RestaurantRequestDto
        {
            Name = "FastFuel Diner",
            Description = "A fast food restaurant serving burgers and fries.",
            Address = "123 Main St, Anytown, USA",
            Latitude = 40.7128,
            Longitude = -74.0060,
            Phone = "555-1234",
            OpeningHours = Enum.GetValues<DayOfWeek>().Select(day => new RestaurantOpeningHourDto
            {
                DayOfWeek = day,
                OpenTime = new TimeOnly(9, 0),
                CloseTime = new TimeOnly(21, 0)
            }).ToList()
        });

        await _stationService.CreateAsync(new StationRequestDto
        {
            Name = "Burger Station 1",
            RestaurantId = restaurant.Id,
            StationCategoryId = burgerStation.Id
        });
        await _stationService.CreateAsync(new StationRequestDto
        {
            Name = "Fries Station 1",
            RestaurantId = restaurant.Id,
            StationCategoryId = friesStation.Id
        });

        await SeedAdmin();
        await SeedEmployee(restaurant.Id);
        await SeedMachine(restaurant.Id);
        var customerId = await SeedCustomer();

        await _orderService.CreateAsync(new OrderRequestDto
        {
            RestaurantId = restaurant.Id,
            Menus =
            [
                new OrderMenuDto
                {
                    MenuId = lunchMenu.Id,
                    Quantity = 1,
                    SpecialInstructions = "Please make the burger without tomato."
                }
            ],
            Foods =
            [
                new OrderFoodDto
                {
                    FoodId = cheeseBurger.Id,
                    Quantity = 1,
                    SpecialInstructions = "Extra cheese, please."
                }
            ]
        }, customerId);
    }

    public async Task SeedProdAsync()
    {
        if ((await _userService.GetAllAsync()).Count != 0) return;

        await SeedAdmin();
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private async Task<uint> SeedEmployee(uint workplaceId)
    {
        var employeeDto = new EmployeeRequestDto
        {
            UserName = "employee",
            Email = "employee@example.com",
            Name = "Employee User",
            Password = "Employee123!",
            ShiftIds = [],
            StationCategoryIds = [],
            WorksAtRestaurantId = workplaceId
        };

        var employee = await _employeeService.CreateAsync(employeeDto);
        return employee.Id;
    }

    private async Task<uint> SeedCustomer()
    {
        var customerDto = new CustomerRequestDto
        {
            UserName = "customer",
            Email = "customer@example.com",
            Name = "Customer User",
            Password = "Customer123!"
        };

        var customer = await _customerService.CreateAsync(customerDto);
        return customer.Id;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private async Task<uint> SeedAdmin()
    {
        var adminRequestDto = new AdminRequestDto
        {
            UserName = "admin",
            Email = "admin@example.com",
            Name = "Admin User",
            Password = "Admin123!"
        };

        var admin = await _adminService.CreateAsync(adminRequestDto);
        return admin.Id;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private async Task<uint> SeedMachine(uint restaurantId)
    {
        var machineRequestDto = new MachineRequestDto
        {
            UserName = "machine",
            LocatedAtRestaurantId = restaurantId,
            Name = "Machine User",
            Password = "Machine123!"
        };

        var machine = await _machineService.CreateAsync(machineRequestDto);
        return machine.Id;
    }
}