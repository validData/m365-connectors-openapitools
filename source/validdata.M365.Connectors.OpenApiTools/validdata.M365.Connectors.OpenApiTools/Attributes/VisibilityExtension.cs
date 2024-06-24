using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

public static class VisibilityExtension
{
    public const string VisibilityExtensionKey = "x-ms-visibility";
    
    public static bool HasVisibility(this OpenApiOperation operation)
    {
        return operation.Extensions.TryGetValue(VisibilityExtensionKey, out _);
    }
    
    
    public static void SetVisibility(this OpenApiOperation operation, MSVisibility visibility)
    {
        operation.Extensions[VisibilityExtensionKey] = new OpenApiString(visibility.ToString().ToLower());
    }
    
}