using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Common.Permissions;

[AttributeUsage(AttributeTargets.Method)]
public class PermissionCheckAttribute : TypeFilterAttribute
{
    public PermissionCheckAttribute(CrudOperation operation)
        : base(typeof(PermissionFilter))
    {
        Operation = operation.ToString();
        Arguments = [operation.ToString()];
    }

    public PermissionCheckAttribute(string operation)
        : base(typeof(PermissionFilter))
    {
        Operation = operation;
        Arguments = [operation];
    }

    public string Operation { get; }
}