using FastFuel.Features.Common;

namespace FastFuel.Features.Themes.Models
{
    public class Theme : IIdentifiable
    {
        public uint Id { get; init; }       
        public string Name { get; set; } = string.Empty;
        public string Background { get; set; } = "#ffffff";
        public string Footer { get; set; } = "#f0f0f0";
        public string ButtonPrimary { get; set; } = "#007bff";
        public string ButtonSecondary { get; set; } = "#6c757d";
    }
}