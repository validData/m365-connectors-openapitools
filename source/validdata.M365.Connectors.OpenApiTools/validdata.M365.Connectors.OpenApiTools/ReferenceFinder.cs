using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace validdata.M365.Connectors.OpenApiTools;

internal static class ReferenceFinder
{
    public static List<OpenApiReference> FindReferences(OpenApiDocument document)
    {
        var toCheck = new Queue<IOpenApiElement>();
        var references = new HashSet<IOpenApiElement>();
        var results = new Dictionary<string, OpenApiReference>();


        void AddToCheck(IOpenApiElement element)
        {
            toCheck.Enqueue(element);
        }

        AddToCheck(document.Paths);


        while (toCheck.Any())
        {
            var current = toCheck.Dequeue();
            if (!references.Add(current))
            {
                continue;
            }

            if (current is OpenApiReference reference)
            {
                results.TryAdd(reference.Id, reference);
                if (document.Components.Schemas.TryGetValue(reference.Id, out var refSchema))
                {
                    AddToCheck(refSchema);
                }
            }

            if (current is OpenApiPaths paths)
            {
                AddEntries(paths, AddToCheck);
            }
            if (current is OpenApiMediaType mediaType)
            {
                AddEntries(mediaType, AddToCheck);
            }

            if (current is OpenApiResponse response)
            {
                AddEntries(response, AddToCheck);
            }

            if (current is OpenApiCallback callback)
            {
                AddEntries(callback, AddToCheck);
            }

            if (current is OpenApiOperation operation)
            {
                AddEntries(operation, AddToCheck);
            }

            if (current is OpenApiObject obj)
            {
                AddEntries(obj, AddToCheck);
            }

            if (current is OpenApiSchema schema)
            {
                AddEntries(schema, AddToCheck);
            }

            if (current is OpenApiPathItem pathItem)
            {
                AddEntries(pathItem, AddToCheck);
            }

            if (current is OpenApiParameter parameter)
            {
                AddEntries(parameter, AddToCheck);
            }

            if (current is IOpenApiExtensible extensible)
            {
                AddEntries(extensible, AddToCheck);
            }

            if (current is IOpenApiReferenceable referenceable)
            {
                AddEntries(referenceable, AddToCheck);
            }
        }

        return results.Values.ToList();
    }

    private static void AddEntries(OpenApiPaths item, Action<IOpenApiElement> toCheck)
    {
        foreach (var (_,path) in item)
        {
            toCheck(path);
        }
    }
    private static void AddEntries(OpenApiMediaType item, Action<IOpenApiElement> toCheck)
    {
        if (item.Schema != null)
        {
            toCheck(item.Schema);
        }
    }

    private static void AddEntries(OpenApiResponse item, Action<IOpenApiElement> toCheck)
    {
        item.Content.Values.ToList().ForEach(toCheck);
    }

    private static void AddEntries(OpenApiCallback item, Action<IOpenApiElement> toCheck)
    {
        item.PathItems.Values.ToList().ForEach(toCheck);
    }

    private static void AddEntries(OpenApiObject item, Action<IOpenApiElement> toCheck)
    {
        foreach (var (_, property) in item)
        {
            toCheck(property);
        }
    }

    private static void AddEntries(OpenApiSchema item, Action<IOpenApiElement> toCheck)
    {
        foreach (var (_, property) in item.Properties)
        {
            toCheck(property);
        }

        if (item.AdditionalProperties != null)
        {
            toCheck(item.AdditionalProperties);
        }

        if (item.Reference != null)
        {
            toCheck(item.Reference);
        }

        if (item.Items != null)
        {
            toCheck(item.Items);
        }
    }

    private static void AddEntries(OpenApiParameter item, Action<IOpenApiElement> toCheck)
    {
        toCheck(item.Schema);
    }

    private static void AddEntries(OpenApiOperation item, Action<IOpenApiElement> toCheck)
    {
        foreach (var openApiParameter in item.Parameters)
        {
            toCheck(openApiParameter);
        }

        item.Responses?.Values.ToList().ForEach(toCheck);
        item.Callbacks?.Values.ToList().ForEach(toCheck);
        if (item.RequestBody != null) toCheck(item.RequestBody);
    }

    private static void AddEntries(OpenApiPathItem item, Action<IOpenApiElement> toCheck)
    {
        foreach (var openApiParameter in item.Parameters)
        {
            toCheck(openApiParameter);
        }

        foreach (var operation in item.Operations)
        {
            toCheck(operation.Value);
        }
    }

    private static void AddEntries(IOpenApiReferenceable item, Action<IOpenApiElement> toCheck)
    {
        if (item.Reference != null)
        {
            toCheck(item.Reference);
        }
    }

    private static void AddEntries(IOpenApiExtensible item, Action<IOpenApiElement> toCheck)
    {
        foreach (var (_, value) in item.Extensions)
        {
            if (value is IOpenApiElement element)
            {
                toCheck(element);
            }
        }
    }
}