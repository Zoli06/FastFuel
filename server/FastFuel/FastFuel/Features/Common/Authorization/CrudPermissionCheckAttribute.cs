namespace FastFuel.Features.Common.Authorization;

public class CrudPermissionCheckAttribute : PermissionCheckAttribute
{
    public CrudPermissionCheckAttribute(PermissionType permission)
        : base(typeof(CrudAuthorizationFilter))
    {
        Permission = permission;
        Arguments = [permission];
    }

    public PermissionType Permission { get; }
}