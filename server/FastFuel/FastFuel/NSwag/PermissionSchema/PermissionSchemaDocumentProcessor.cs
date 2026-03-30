using FastFuel.Features.Permissions.Services;
using NJsonSchema;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace FastFuel.NSwag.PermissionSchema;

public class PermissionSchemaDocumentProcessor(IServiceProvider serviceProvider) : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        // Create a scope to resolve the scoped PermissionService
        using var scope = serviceProvider.CreateScope();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        // Get all permissions from the service
        var permissions = permissionService.GetAllPermissionsAsync().GetAwaiter().GetResult();

        // Find all permission endpoints
        foreach (var pathItem in context.Document.Paths)
        {
            if (!pathItem.Key.Contains("/Permission"))
                continue;

            foreach (var operation in pathItem.Value.Values)
            {
                if (operation == null) continue;

                // Check for 200 response
                if (!operation.Responses.TryGetValue("200", out var response))
                    continue;

                // Check if it returns an array
                if (!response.Content.TryGetValue("application/json", out var mediaType))
                    continue;

                var schema = mediaType.Schema?.ActualSchema;
                if (schema == null || schema.Type != JsonObjectType.Array)
                    continue;

                // Get or create the items schema
                if (schema.Item == null)
                {
                    schema.Item = new JsonSchema
                    {
                        Type = JsonObjectType.String
                    };
                }

                var itemSchema = schema.Item.ActualSchema;

                // Add enum values
                itemSchema.Enumeration.Clear();
                foreach (var permission in permissions)
                {
                    itemSchema.Enumeration.Add(permission);
                }

                itemSchema.Type = JsonObjectType.String;
                itemSchema.Description = "A permission string in the format 'Permission:{Controller}:{Operation}'";
                itemSchema.Example = permissions.FirstOrDefault();

                schema.Description = $"List of permission strings. Permissions follow the format 'Permission:{{Controller}}:{{Operation}}'. " +
                                   $"Currently {permissions.Count} permissions are available in the system.";
            }
        }
    }
}