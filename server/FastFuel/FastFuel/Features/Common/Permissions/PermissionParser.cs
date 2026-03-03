namespace FastFuel.Features.Common.Permissions;

public static class PermissionParser
{
    public static string ParsePermissionName(string group, string operation)
    {
        return $"Permission:{group}:{operation}";
    }
}