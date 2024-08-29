using System.Net.Mime;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class AddPlainBodyParameterAttribute() : MSBaseAttribute
{
    public override void ApplyOperation(OperationFilterContext context, OpenApiOperation operation)
    {
        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [MediaTypeNames.Text.Plain] = new()
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                }
            }
        };
    }
}