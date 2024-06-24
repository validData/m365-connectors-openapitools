using Microsoft.OpenApi.Models;

namespace validdata.M365.Connectors.OpenApiTools;

public class DocumentSchemaFilterContext
{
    private readonly List<OpenApiReference> _references = [];

    public IReadOnlyList<OpenApiReference> References => _references; 
    
    public void AddSchemaReference(OpenApiReference reference)
    {
        _references.Add(reference);
    }
    
}