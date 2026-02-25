namespace FastFuel.Features.Common.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SkipPermissionCheckAttribute : Attribute;