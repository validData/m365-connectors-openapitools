using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MSDynamicResponseSchemaAttribute(string operationId, string description, string responseMimeType, params string[] parameterName)
    : MSBaseAttribute
{
    private string SchemaName => $"{OperationId}OperationSchema";
   
    
    // ReSharper disable MemberCanBePrivate.Global
    public string OperationId { get; } = operationId;
    public string Description { get; } = description;
    public string[] ParameterName { get; } = parameterName;
    // ReSharper restore MemberCanBePrivate.Global


    public override void ApplyOperation(OperationFilterContext context, OpenApiOperation operation)
    {
        operation.Responses["200"].Content[responseMimeType] = new OpenApiMediaType
        {
            Schema = new OpenApiSchema
            {
                Reference = new OpenApiReference
                {
                    Id = SchemaName,
                    Type = ReferenceType.Schema
                }
            }
        };
    }

    public override void ApplyDocument(OpenApiDocument document, DocumentFilterContext context,
        DocumentSchemaFilterContext filterContext)
    {
        var openApiExtension = new OpenApiObject
        {
            ["operationId"] = new OpenApiString(OperationId),
        };

        if (ParameterName.Any())
        {
            var parametersObject = new OpenApiObject();
            foreach (var se in ParameterName)
            {
                parametersObject[se] = new OpenApiObject
                {
                    ["parameter"] = new OpenApiString(se)
                };
            }

            openApiExtension["parameters"] = parametersObject;
        }

        document.Components.Schemas.Add(SchemaName, new OpenApiSchema
        {
            Type = "object",
            Description = Description,
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                ["x-ms-dynamic-schema"] = openApiExtension
            }
        });
    }
}