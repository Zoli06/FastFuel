namespace FastFuel.Features.Common.Permissions;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SkipPermissionCheckAttribute : Attribute;