using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.WebUtilities;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace FastFuel.NSwag.UnregisteredStatusCodeResultOperation;

/// <summary>
///     Adds response entries for every result type in the return signature that:
///     • implements IStatusCodeHttpResult  (so we can read the status code), and
///     • does NOT implement IEndpointMetadataProvider  (types that do are already
///     handled by NSwag's built-in pipeline).
///     Works with arbitrary nesting: Task&lt;Results&lt;Ok, UnauthorizedHttpResult&gt;&gt;, etc.
/// </summary>
public class UnregisteredStatusCodeResultOperationProcessor : IOperationProcessor
{
    private static readonly Dictionary<Type, int> StatusCodeOverrides = new()
    {
        { typeof(ForbidHttpResult), 403 }
    };

    public bool Process(OperationProcessorContext context)
    {
        var returnType = context.MethodInfo?.ReturnType;
        if (returnType is null) return true;

        foreach (var type in CollectLeafTypes(returnType))
        {
            // Skip types NSwag already knows about
            if (type.IsAssignableTo(typeof(IEndpointMetadataProvider))) continue;
            // Only care about types that carry a status code
            if (!type.IsAssignableTo(typeof(IStatusCodeHttpResult))
                && !StatusCodeOverrides.ContainsKey(type)) continue;

            var statusCode = ResolveStatusCode(type);
            if (statusCode is null) continue;

            var key = statusCode.Value.ToString();
            if (!context.OperationDescription.Operation.Responses.ContainsKey(key))
                context.OperationDescription.Operation.Responses.Add(key, new OpenApiResponse
                {
                    Description = ReasonPhrases.GetReasonPhrase(statusCode.Value)
                });
        }

        return true;
    }

    /// <summary>
    ///     Recursively unwraps Task&lt;&gt;, ValueTask&lt;&gt;, Results&lt;,&gt;, and arrays,
    ///     yielding only the concrete leaf types.
    /// </summary>
    private static IEnumerable<Type> CollectLeafTypes(Type type)
    {
        if (type.IsGenericType)
        {
            foreach (var arg in type.GetGenericArguments())
                foreach (var leaf in CollectLeafTypes(arg))
                    yield return leaf;

            yield break;
        }

        if (type.IsArray)
        {
            var element = type.GetElementType();
            if (element is not null)
                foreach (var leaf in CollectLeafTypes(element))
                    yield return leaf;

            yield break;
        }

        yield return type;
    }

    /// <summary>
    ///     Instantiates the type (including non-public constructors) and reads
    ///     IStatusCodeHttpResult.StatusCode. Returns null if instantiation fails.
    /// </summary>
    private static int? ResolveStatusCode(Type type)
    {
        if (StatusCodeOverrides.TryGetValue(type, out var overrideCode))
            return overrideCode;

        try
        {
            return Activator.CreateInstance(type, true) is IStatusCodeHttpResult r
                ? r.StatusCode
                : null;
        }
        catch
        {
            return null;
        }
    }
}