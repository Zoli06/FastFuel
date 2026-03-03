using System.Reflection;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace FastFuel.NSwag.SwaggerQueryParam;

public class SwaggerQueryParamProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var method = context.MethodInfo;

        var attrs = method
            .GetCustomAttributes<SwaggerQueryParamAttribute>()
            .Concat(method.GetBaseDefinition()
                .GetCustomAttributes<SwaggerQueryParamAttribute>());

        foreach (var attr in attrs)
        {
            var schema = context.SchemaGenerator.Generate(attr.Type, context.SchemaResolver);

            context.OperationDescription.Operation.Parameters.Add(
                new OpenApiParameter
                {
                    Name = attr.Name,
                    Kind = OpenApiParameterKind.Query,
                    IsRequired = attr.Required,
                    Schema = schema
                });
        }

        return true;
    }
}