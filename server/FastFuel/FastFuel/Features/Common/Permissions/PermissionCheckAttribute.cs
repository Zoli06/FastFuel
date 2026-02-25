using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Common.Permissions;

[AttributeUsage(AttributeTargets.Method)]
public abstract class PermissionCheckAttribute(Type type) : TypeFilterAttribute(type);