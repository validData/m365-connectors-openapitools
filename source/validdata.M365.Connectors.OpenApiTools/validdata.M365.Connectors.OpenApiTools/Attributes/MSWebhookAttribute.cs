using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class MSWebhookAttribute(string operationId, Type payloadType, string? description = null) : MSBaseAttribute
{
    private readonly string _schemaName = $"webhook_{payloadType.Name}";

    public override void ApplyOperation(OperationFilterContext context, OpenApiOperation operation)
    {
        operation.OperationId = operationId;
        operation.Extensions["x-ms-trigger"] = new OpenApiString("single");
        base.ApplyOperation(context, operation);
    }

    public override void ApplyDocument(OpenApiDocument document, DocumentFilterContext context,
        DocumentSchemaFilterContext filterContext)
    {
        var openApiPaths = document.Paths.ToList();
        foreach (var (key,path) in openApiPaths)
        {
            if (path.Operations.All(x => x.Value.OperationId != operationId)) continue;
            var payloadObject = new OpenApiObject
            {
                ["schema"] = new OpenApiObject
                {
                    ["$ref"] = new OpenApiString($"#/definitions/{_schemaName}" )
                }
            };
            if (!string.IsNullOrWhiteSpace(description))
            {
                payloadObject["description"] = new OpenApiString(description);
            }
            path.Extensions["x-ms-notification-content"] = payloadObject;
        }

        var openApiSchema = context.SchemaGenerator.GenerateSchema(payloadType, context.SchemaRepository);
        var generatedSchemaId = openApiSchema.Reference.Id;
        var schemaGenerated = document.Components.Schemas[generatedSchemaId];
        document.Components.Schemas[_schemaName] = schemaGenerated;
        document.Components.Schemas.Remove(generatedSchemaId);
        filterContext.AddSchemaReference(new OpenApiReference{Id = _schemaName, Type = ReferenceType.Schema});
        base.ApplyDocument(document,context,filterContext);
    }
}