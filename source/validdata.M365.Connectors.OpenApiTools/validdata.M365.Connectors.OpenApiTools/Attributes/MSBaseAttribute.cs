using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

public abstract class MSBaseAttribute : Attribute
{
    
    public virtual void ApplyDocument(OpenApiDocument document, DocumentFilterContext context,
        DocumentSchemaFilterContext filterContext)
    {
        
    }
    
    public virtual void ApplyOperation(OperationFilterContext context, OpenApiOperation operation)
    {
        
    }
}
