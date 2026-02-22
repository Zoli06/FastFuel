using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Themes.Models;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Users.Models;

public class User : IdentityUser<uint>, IIdentifiable
{
    public string Name { get; set; } = string.Empty;

    public new string Email
    {
        get => base.Email ?? string.Empty;
        set => base.Email = value;
    }

    public new string UserName
    {
        get => base.UserName ?? string.Empty;
        set => base.UserName = value;
    }

    public new string PasswordHash
    {
        get => base.PasswordHash ?? string.Empty;
        set => base.PasswordHash = value;
    }

    public uint? ThemeId { get; set; }
    public virtual Theme? Theme { get; set; }
}