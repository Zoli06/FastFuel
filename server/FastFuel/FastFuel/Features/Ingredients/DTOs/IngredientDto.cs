namespace FastFuel.Features.Ingredients.DTOs
{
    public class IngredientDto
    {
        public uint Id { get; set; }
        public required string Name { get; set; }
        public uint StationTypeId { get; set; }
        public Uri? ImageUrl { get; set; }
    }
}
