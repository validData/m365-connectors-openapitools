using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools;

internal class SetupOptions : ISetupOptions
{
    public required string Host { get; init; }
    public string? BasePath { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Version { get; set; } = "v1";
    public Assembly? XmlCommentsAssembly { get; private set; }
    public bool AuthenticateWithAzureAdDefaultsEnabled { get; private set; }
    public bool RemoveOperationsWithoutVisibilityAttributeEnabled { get; private set; }
    public Action<SwaggerGenOptions>? SwaggerGenAction { get; private set; }
    
    public void CustomizeSwaggerGen(Action<SwaggerGenOptions> setup)
    {
        SwaggerGenAction = setup;
    }
    
    public void AuthenticateWithAzureAdDefaults()
    {
        AuthenticateWithAzureAdDefaultsEnabled = true;
    }

    public void RemoveOperationsWithoutVisibilityAttribute()
    {
        RemoveOperationsWithoutVisibilityAttributeEnabled = true;
    }
    
    public void IncludeXmlCommentsForAssembly(Assembly assembly)
    {
        XmlCommentsAssembly = assembly;
    }
    
}