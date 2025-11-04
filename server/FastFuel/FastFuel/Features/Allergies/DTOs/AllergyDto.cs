namespace FastFuel.Features.Allergies.DTOs
{
    public class AllergyDto
    {
        public uint Id { get; set; }
        public required string Name { get; set; }
        public string? Message { get; set; }
    }
}
