using System.Reflection;
using FastFuel.Features.Common.Permissions;
using FastFuel.Features.Customers.Controllers;
using FastFuel.Features.Orders.Controllers;

namespace FastFuel.Tests;

public class PermissionRequirementResolverTests
{
    [Fact]
    public void ResolveRequiredPermission_UsesCrudPermissionForInheritedAction()
    {
        var method = GetMethod(typeof(CustomerController), "Delete");

        var permission = PermissionRequirementResolver.ResolveRequiredPermission(method, typeof(CustomerController));

        Assert.Equal("Permission:Customer:Delete", permission);
    }

    [Fact]
    public void ResolveRequiredPermission_UsesCustomOperationPermission()
    {
        var method = GetMethod(typeof(CustomerController), "UpdateSelf");

        var permission = PermissionRequirementResolver.ResolveRequiredPermission(method, typeof(CustomerController));

        Assert.Equal("Permission:Customer:UpdateSelf", permission);
    }

    [Fact]
    public void ResolveRequiredPermission_ReturnsNullForAllowAnonymousOverride()
    {
        var method = GetMethod(typeof(CustomerController), "Create");

        var permission = PermissionRequirementResolver.ResolveRequiredPermission(method, typeof(CustomerController));

        Assert.Null(permission);
    }

    [Fact]
    public void ResolveRequiredPermission_ReturnsNullWhenPermissionCheckIsMissing()
    {
        var method = GetMethod(typeof(OrderController), "GetMyOrders");

        var permission = PermissionRequirementResolver.ResolveRequiredPermission(method, typeof(OrderController));

        Assert.Null(permission);
    }

    private static MethodInfo GetMethod(Type controllerType, string methodName)
    {
        return controllerType.GetMethod(methodName)
               ?? throw new InvalidOperationException($"Method '{methodName}' not found on '{controllerType.Name}'.");
    }
}