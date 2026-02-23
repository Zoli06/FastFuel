using FastFuel.Features.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Roles.Entities;

public class Role : IdentityRole<uint>, IIdentifiable
{
    public new string Name
    {
        get => base.Name ?? string.Empty;
        set => base.Name = value;
    }
}