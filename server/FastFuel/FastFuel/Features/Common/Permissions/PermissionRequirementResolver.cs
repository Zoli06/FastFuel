using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace FastFuel.Features.Common.Permissions;

public static class PermissionRequirementResolver
{
    public static string? ResolveRequiredPermission(MethodInfo methodInfo, Type controllerType)
    {
        if (HasAttributeOnMethodOrBase<IAllowAnonymous>(methodInfo)
            || HasAttributeOnTypeHierarchy<IAllowAnonymous>(controllerType)
            || HasAttributeOnMethodOrBase<SkipPermissionCheckAttribute>(methodInfo)
            || HasAttributeOnTypeHierarchy<SkipPermissionCheckAttribute>(controllerType))
            return null;

        var permissionCheck = methodInfo.GetCustomAttribute<PermissionCheckAttribute>()
                              ?? methodInfo.GetBaseDefinition().GetCustomAttribute<PermissionCheckAttribute>();

        if (permissionCheck == null)
            return null;

        var controllerName = GetControllerName(controllerType);
        return PermissionParser.ParsePermissionName(controllerName, permissionCheck.Operation);
    }

    private static bool HasAttributeOnMethodOrBase<TAttribute>(MethodInfo methodInfo) where TAttribute : class
    {
        return methodInfo.GetCustomAttributes().OfType<TAttribute>().Any()
               || methodInfo.GetBaseDefinition().GetCustomAttributes().OfType<TAttribute>().Any();
    }

    private static bool HasAttributeOnTypeHierarchy<TAttribute>(Type type) where TAttribute : class
    {
        for (var current = type; current != null; current = current.BaseType)
            if (current.GetCustomAttributes().OfType<TAttribute>().Any())
                return true;

        return false;
    }

    private static string GetControllerName(Type type)
    {
        const string suffix = "Controller";

        if (type.Name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            return type.Name[..^suffix.Length];

        return type.Name;
    }
}