using System.Text;
using FastFuel.Features.Common.Controllers;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace FastFuel.NSwag.CrudOperationSummary;

public class CrudOperationSummaryOperationProcessor : IOperationProcessor
{
    private const string GetAllMethodName = "GetAll";
    private const string GetByIdMethodName = "GetById";
    private const string CreateMethodName = "Create";
    private const string UpdateMethodName = "Update";
    private const string DeleteMethodName = "Delete";

    public bool Process(OperationProcessorContext context)
    {
        var methodInfo = context.MethodInfo;
        var controllerType = context.ControllerType;

        if (!IsCrudAction(methodInfo, controllerType))
            return true;

        var entityType = ResolveCrudEntityType(controllerType);

        if (entityType != null)
        {
            var entityName = ToSentenceCase(entityType.Name);
            context.OperationDescription.Operation.Summary = methodInfo.Name switch
            {
                GetAllMethodName => $"Gets all {entityName}s.",
                GetByIdMethodName => $"Gets one {entityName}.",
                CreateMethodName => $"Creates a new {entityName}.",
                UpdateMethodName => $"Updates one {entityName}.",
                DeleteMethodName => $"Deletes one {entityName}.",
                _ => context.OperationDescription.Operation.Summary
            };
        }

        return true;
    }

    private static bool IsCrudAction(System.Reflection.MethodInfo methodInfo, Type controllerType)
    {
        var crudControllerType = FindCrudControllerBaseType(controllerType);
        if (crudControllerType == null)
            return false;

        return methodInfo.DeclaringType?.IsGenericType == true
               && methodInfo.DeclaringType.GetGenericTypeDefinition() == typeof(CrudController<,,>);
    }

    private static Type? ResolveCrudEntityType(Type controllerType)
    {
        return FindCrudControllerBaseType(controllerType)?.GetGenericArguments()[0];
    }

    private static Type? FindCrudControllerBaseType(Type controllerType)
    {
        var currentType = controllerType;
        while (currentType != null)
        {
            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(CrudController<,,>))
                return currentType;

            currentType = currentType.BaseType;
        }

        return null;
    }

    private static string ToSentenceCase(string value)
    {
        var builder = new StringBuilder(value.Length + 8);
        builder.Append(char.ToLowerInvariant(value[0]));

        for (var i = 1; i < value.Length; i++)
        {
            var current = value[i];
            var previous = value[i - 1];
            var next = i + 1 < value.Length ? value[i + 1] : '\0';

            var startsNewWord =
                char.IsUpper(current) &&
                (char.IsLower(previous) || (char.IsUpper(previous) && next != '\0' && char.IsLower(next)));

            if (startsNewWord)
                builder.Append(' ');

            builder.Append(char.ToLowerInvariant(current));
        }

        return builder.ToString();
    }
}