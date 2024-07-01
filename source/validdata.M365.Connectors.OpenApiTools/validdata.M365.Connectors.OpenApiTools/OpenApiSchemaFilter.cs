using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using validdata.M365.Connectors.OpenApiTools.Attributes;

namespace validdata.M365.Connectors.OpenApiTools;

public class OpenApiSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
        {
            return;
        }

        foreach (var property in schema.Properties)
        {
            var attributes = context.Type
                .GetProperty(property.Key,BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                ?.GetCustomAttributes()
                .ToList();
            if (attributes == null) continue;
            foreach (var attribute in attributes)
            {
                if (attribute is MSBaseAttribute baseAttribute)
                {
                    baseAttribute.ApplyProperty(property.Value);
                }
            }
        }
    }
}