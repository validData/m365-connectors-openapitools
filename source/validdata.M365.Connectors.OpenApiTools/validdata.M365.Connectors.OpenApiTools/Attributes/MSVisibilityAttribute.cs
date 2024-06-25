using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class MSVisibilityAttribute(MSVisibility visibility) : MSBaseAttribute
{
    
    // ReSharper disable once MemberCanBePrivate.Global
    public MSVisibility Visibility { get; } = visibility;

    public override void ApplyOperation(OperationFilterContext context, OpenApiOperation operation)
    {
        operation.SetVisibility(Visibility);
        base.ApplyOperation(context, operation);
    }
}