using FastFuel.Features.Common.Interfaces;
using FastFuel.Features.Orders.Entities;
using Microsoft.AspNetCore.Identity;

namespace FastFuel.Features.Users.Entities;

public class User : IdentityUser<uint>, IIdentifiable
{
    public string Name { get; set; } = string.Empty;


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

    public virtual List<Order> Orders { get; init; } = [];
}