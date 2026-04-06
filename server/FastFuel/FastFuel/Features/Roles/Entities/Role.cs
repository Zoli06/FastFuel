using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Pages.Common;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Roles.Entities;

public class Role : IdentityRole<uint>, IIdentifiable
{
    public new string Name
    {
        get => base.Name ?? string.Empty;
        set => base.Name = value;
    }

    public bool IsDefault { get; set; }
    public bool ArePermissionsImmutable { get; set; }
    public List<Page> Pages { get; set; } = [];
}