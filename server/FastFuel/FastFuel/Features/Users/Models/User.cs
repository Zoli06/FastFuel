using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Themes.Models;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Users.Models;

public class User : IdentityUser<uint>, IIdentifiable
{
    public string Name { get; set; } = string.Empty;
    public new virtual string Email { get; set; } = string.Empty;
    public new virtual string UserName { get; set; } = string.Empty;
    public new virtual string PasswordHash { get; set; } = string.Empty;
    public uint ThemeId { get; set; }
    public virtual Theme Theme { get; set; } = null!;
}