using Microsoft.AspNetCore.Mvc;

namespace FastFuel.Features.Common.Authorization;

[AttributeUsage(AttributeTargets.Method)]
public abstract class PermissionCheckAttribute(Type type) : TypeFilterAttribute(type);