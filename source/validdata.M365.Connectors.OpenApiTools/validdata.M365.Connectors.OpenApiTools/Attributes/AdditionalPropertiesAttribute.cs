using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class AdditionalPropertiesAttribute(bool allowAdditionalProperties) : MSBaseAttribute
{
    public override void ApplySchema(OpenApiSchema schema)
    {
        schema.AdditionalPropertiesAllowed = allowAdditionalProperties;
    }
}
