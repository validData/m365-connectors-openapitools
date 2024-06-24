using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using validdata.M365.Connectors.OpenApiTools.Attributes;

namespace validdata.M365.Connectors.OpenApiTools;

internal class OpenApiOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo.GetCustomAttributes().ToList();
        foreach (var attribute in attributes)
        {
            if (attribute is MSBaseAttribute baseAttribute)
            {
                baseAttribute.ApplyOperation(context,operation);
            }
        }
    }
}