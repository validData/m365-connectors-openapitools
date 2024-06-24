using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools;

public interface ISetupOptions
{
    string Host { get; }
    string? BasePath { get; set; }
    string? Title { get; set; }
    void IncludeXmlCommentsForAssembly(Assembly assembly);
    void AuthenticateWithAzureAdDefaults();
    void RemoveOperationsWithoutVisibilityAttribute();
    void CustomizeSwaggerGen(Action<SwaggerGenOptions> setup);
}