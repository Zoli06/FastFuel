using FastFuel.Features.Common.Permissions;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace FastFuel.NSwag.PermissionSchema;

public class PermissionSchemaOperationProcessor : IOperationProcessor
{
    private const string RequiredPermissionKey = "x-required-permission";
    private const string RequiredPermissionPrefix = "**Required permission:**";

    public bool Process(OperationProcessorContext context)
    {
        var methodInfo = context.MethodInfo;
        var controllerType = context.ControllerType;
        if (methodInfo == null || controllerType == null)
            return true;

        var requiredPermission = PermissionRequirementResolver.ResolveRequiredPermission(methodInfo, controllerType);

        context.OperationDescription.Operation.ExtensionData ??= new Dictionary<string, object?>();
        if (requiredPermission == null)
        {
            context.OperationDescription.Operation.ExtensionData.Remove(RequiredPermissionKey);
            return true;
        }

        context.OperationDescription.Operation.ExtensionData[RequiredPermissionKey] = requiredPermission;

        var operation = context.OperationDescription.Operation;
        var requiredPermissionLine = $"{RequiredPermissionPrefix} `{requiredPermission}`";

        if (string.IsNullOrWhiteSpace(operation.Description))
            operation.Description = requiredPermissionLine;
        else if (!operation.Description.Contains(requiredPermissionLine, StringComparison.Ordinal))
            operation.Description = $"{operation.Description}\n\n{requiredPermissionLine}";

        return true;
    }
}