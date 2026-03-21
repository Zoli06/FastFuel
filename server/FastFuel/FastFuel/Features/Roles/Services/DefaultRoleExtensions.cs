namespace FastFuel.Features.Roles.Services;

public static class DefaultRoleExtensions
{
    public static string ToRoleName(this DefaultRole role)
    {
        return role switch
        {
            DefaultRole.User => "User (shared base)",
            _ => role.ToString()
        };
    }
}