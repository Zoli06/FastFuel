namespace FastFuel.Features.Common.Permissions.Crud;

public class CrudPermissionCheckAttribute : PermissionCheckAttribute
{
    public CrudPermissionCheckAttribute(CrudPermissionType crudPermission)
        : base(typeof(CrudAuthorizationFilter))
    {
        CrudPermission = crudPermission;
        Arguments = [crudPermission];
    }

    public CrudPermissionType CrudPermission { get; }
}