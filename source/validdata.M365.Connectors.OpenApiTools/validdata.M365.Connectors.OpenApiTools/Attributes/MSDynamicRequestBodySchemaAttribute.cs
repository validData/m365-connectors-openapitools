using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class MSDynamicRequestBodySchemaAttribute(
    string schemaOperationId,
    string description,
    string? valuePath,
    params string[] parameterMappings)
    : MSBaseAttribute
{
    // ReSharper disable MemberCanBePrivate.Global
    public string Description { get; } = description;
    public string SchemaOperationId { get; } = schemaOperationId;
    public string? ValuePath { get; } = valuePath;
    public string[] ParameterMappings { get; } = parameterMappings;
    // ReSharper restore MemberCanBePrivate.Global

    public override void ApplyOperation(OperationFilterContext context, OpenApiOperation operation)
    {
        var openApiObject = new OpenApiObject();

        foreach (var mapping in ParameterMappings)
        {
            openApiObject[mapping] = new OpenApiObject
            {
                ["parameter"] = new OpenApiString(mapping)
            };
        }

        var openApiExtension = new OpenApiObject
        {
            ["operationId"] = new OpenApiString(SchemaOperationId),
            ["parameters"] = openApiObject
        };
        if (!string.IsNullOrWhiteSpace(ValuePath))
        {
            openApiExtension["value-path"] = new OpenApiString(ValuePath);
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Description = Description,
                        Extensions = new Dictionary<string, IOpenApiExtension>
                        {
                            ["x-ms-dynamic-schema"] = openApiExtension
                        }
                    }
                }
            }
        };
    }
}