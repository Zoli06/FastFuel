namespace FastFuel.Features.Common.Authorization;

public class CrudAuthorizeAttribute : PermissionAuthorizeAttribute
{
    public CrudAuthorizeAttribute(PermissionType permission)
        : base(typeof(CrudAuthorizationFilter))
    {
        Arguments = [permission];
    }
}