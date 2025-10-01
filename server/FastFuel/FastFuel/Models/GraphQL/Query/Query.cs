namespace FastFuel.Models.GraphQL;

public class Query
{
    public Food GetFoodById(uint id, [Service] ApplicationDbContext context)
    {
        return context.Foods.First(f => f.Id == id);
    }
}