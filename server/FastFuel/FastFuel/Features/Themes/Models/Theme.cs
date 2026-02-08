using FastFuel.Features.Common.Interfaces;

namespace FastFuel.Features.Themes.Models;

public class Theme : IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public string Background { get; set; } = string.Empty;
    public string Footer { get; set; } = string.Empty;
    public string ButtonPrimary { get; set; } = string.Empty;
    public string ButtonSecondary { get; set; } = string.Empty;
    public uint Id { get; init; }
}