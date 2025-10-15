namespace FastFuel.Models.GraphQL;

public class Query
{
    public IQueryable<Menu> GetMenus([Service] ApplicationDbContext context)
    {
        return context.Menus;
    }
    
    public Menu GetMenuById([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint id, [Service] ApplicationDbContext context)
    {
        return context.Menus.First(m => m.Id == id);
    }
    
    public IQueryable<Food> GetFoods([Service] ApplicationDbContext context)
    {
        return context.Foods;
    }
    
    public Food GetFoodById([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint id, [Service] ApplicationDbContext context)
    {
        return context.Foods.First(f => f.Id == id);
    }
    
    public IQueryable<Ingredient> GetIngredients([Service] ApplicationDbContext context)
    {
        return context.Ingredients;
    }
    
    public Ingredient GetIngredientById([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint id, [Service] ApplicationDbContext context)
    {
        return context.Ingredients.First(i => i.Id == id);
    }
    
    public IQueryable<Allergy> GetAllergies([Service] ApplicationDbContext context)
    {
        return context.Allergies;
    }
    
    public Allergy GetAllergyById([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint id, [Service] ApplicationDbContext context)
    {
        return context.Allergies.First(a => a.Id == id);
    }
    
    public IQueryable<StationCategory> GetStationCategories([Service] ApplicationDbContext context)
    {
        return context.StationCategories;
    }
    
    public StationCategory GetStationCategoryById([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint id, [Service] ApplicationDbContext context)
    {
        return context.StationCategories.First(sc => sc.Id == id);
    }
    
    public IQueryable<Restaurant> GetRestaurants([Service] ApplicationDbContext context)
    {
        return context.Restaurants;
    }
    
    public Restaurant GetRestaurantById([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint id, [Service] ApplicationDbContext context)
    {
        return context.Restaurants.First(r => r.Id == id);
    }
    
    public IQueryable<Station> GetStations([Service] ApplicationDbContext context)
    {
        return context.Stations;
    }
    
    public Station GetStationById([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint id, [Service] ApplicationDbContext context)
    {
        return context.Stations.First(s => s.Id == id);
    }
    
    public IQueryable<Order> GetOrders([Service] ApplicationDbContext context)
    {
        return context.Orders;
    }
    
    public Order GetOrderById([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint id, [Service] ApplicationDbContext context)
    {
        return context.Orders.First(o => o.Id == id);
    }
    
    public OpeningHour GetOpeningHoursByRestaurantId([GraphQLType(typeof(IdType)), GraphQLNonNullType] uint restaurantId, [Service] ApplicationDbContext context)
    {
        return context.OpeningHours.First(oh => oh.RestaurantId == restaurantId);
    }
}