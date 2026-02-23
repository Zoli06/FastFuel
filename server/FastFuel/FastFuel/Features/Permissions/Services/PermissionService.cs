using System.Reflection;
using FastFuel.Features.Common.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Permissions.Services;

public class PermissionService : IPermissionService
{
    private readonly IReadOnlyList<string> _permissions;

    public PermissionService()
    {
        var operations = Enum.GetNames<PermissionType>();

        _permissions = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.IsAssignableTo(typeof(ControllerBase))
                        && t.GetMethods().Any(m => m.IsDefined(typeof(PermissionAuthorizeAttribute), true)))
            .Select(t => t.Name.Replace("Controller", ""))
            .SelectMany(resource => operations.Select(op => $"Permission:{resource}:{op}"))
            .OrderBy(p => p)
            .ToList()
            .AsReadOnly();
    }

    public List<string> GetAllPermissions()
    {
        return _permissions.ToList();
    }
}