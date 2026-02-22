using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Common.Authorization;

public abstract class PermissionAuthorizeAttribute(Type type) : TypeFilterAttribute(type);