using FastFuel.Features.Users.Entities;

namespace FastFuel.Features.Admins.Entities;

public class Admin : User
{
    public new string Email
    {
        get => base.Email ?? string.Empty;
        set => base.Email = value;
    }
}