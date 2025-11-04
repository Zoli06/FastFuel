namespace FastFuel.Features.Ingredients.DTOs
{
    public class EditIngredientDto
    {
        public required string Name { get; set; }
        public uint StationTypeId { get; set; }
        public Uri? ImageUrl { get; set; }
    }
}
