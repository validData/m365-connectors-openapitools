using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using validdata.M365.Connectors.OpenApiTools.Attributes;

namespace validdata.M365.Connectors.OpenApiTools;

internal class OpenApiDocumentFilter(SetupOptions configuration) : IDocumentFilter
{

    public void Apply(OpenApiDocument doc, DocumentFilterContext context)
    {
        var filterContext = new DocumentSchemaFilterContext();
        
        doc.Extensions.Add("host", new OpenApiString(configuration.Host));
        doc.Extensions.Add("produces", new OpenApiArray {new OpenApiString("application/json")});
        doc.Extensions.Add("schemes", new OpenApiArray {new OpenApiString("https")});

        if (!string.IsNullOrWhiteSpace(configuration.BasePath))
        {
            doc.Extensions.Add("basePath", new OpenApiString(configuration.BasePath));
        }
        
        ApplyAttributes(doc, context, filterContext);

        if (configuration.RemoveOperationsWithoutVisibilityAttributeEnabled)
        {
            FilterPathsWithOperationsWithoutVisibilityAttribute(doc);
        }

        RemoveSchemasWithoutAnyReference(doc, filterContext);
        
    }

    private static void ApplyAttributes(OpenApiDocument doc, DocumentFilterContext context,
        DocumentSchemaFilterContext filterContext)
    {
        foreach (var apiDescription in context.ApiDescriptions)
        {
            if (!apiDescription.TryGetMethodInfo(out var methodInfo)) continue;
            foreach (var customAttribute in methodInfo.GetCustomAttributes(true))
            {
                if (customAttribute is MSBaseAttribute baseAttribute)
                {
                    baseAttribute.ApplyDocument(doc, context, filterContext);
                }
            }
        }
    }

    private static void RemoveSchemasWithoutAnyReference(OpenApiDocument doc, DocumentSchemaFilterContext filterContext)
    {
        var schemaKeys = doc.Components.Schemas.Keys.ToList();
        var references = ReferenceFinder.FindReferences(doc);
        foreach (var key in schemaKeys)
        {
            if (filterContext.References.Any(x => x.Id.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
            {
                continue;
            }

            if (references.Any(x => x.Id == key))
            {
                continue;
            }
            doc.Components.Schemas.Remove(key);
        }
    }

    private static void FilterPathsWithOperationsWithoutVisibilityAttribute(OpenApiDocument doc)
    {
        var pathKeys = doc.Paths.Keys;
        foreach (var key in pathKeys)
        {
            var openApiPathItem = doc.Paths[key];

            var operations = openApiPathItem.Operations.ToList();
            foreach (var entry in operations)
            {
                if (!entry.Value.HasVisibility())
                {
                    openApiPathItem.Operations.Remove(entry.Key);
                }
            }
            if(openApiPathItem.Operations.Count == 0)
            {
                doc.Paths.Remove(key);
            }
        }
    }
}