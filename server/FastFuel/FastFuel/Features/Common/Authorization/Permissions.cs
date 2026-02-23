namespace FastFuel.Features.Common.Authorization;

public static class Permissions
{
    public static string Create(string resource)
    {
        return $"Permission:{resource}:Create";
    }

    public static string Read(string resource)
    {
        return $"Permission:{resource}:Read";
    }

    public static string Update(string resource)
    {
        return $"Permission:{resource}:Update";
    }

    public static string Delete(string resource)
    {
        return $"Permission:{resource}:Delete";
    }
}