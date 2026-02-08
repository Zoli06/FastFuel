using Microsoft.AspNetCore.Http.HttpResults;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace FastFuel.NSwag
{
    // Adds a 401 response to operations whose return type contains UnauthorizedHttpResult
    public class UnauthorizedHttpResultOperationProcessor : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var returnType = context.MethodInfo?.ReturnType;
            if (returnType == null) return true;

            if (!ContainsUnauthorizedHttpResult(returnType)) return true;
            if (!context.OperationDescription.Operation.Responses.ContainsKey("401"))
            {
                context.OperationDescription.Operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            }

            return true;
        }

        private static bool ContainsUnauthorizedHttpResult(Type type)
        {
            // direct match
            if (type == typeof(UnauthorizedHttpResult)) return true;

            // unwrap Task<> / ValueTask<> / other generics
            if (type.IsGenericType)
            {
                if (type.GetGenericArguments().Any(ContainsUnauthorizedHttpResult))
                {
                    return true;
                }
            }

            // arrays
            if (!type.IsArray) return false;
            var elem = type.GetElementType();
            return elem != null && ContainsUnauthorizedHttpResult(elem);

        }
    }
}