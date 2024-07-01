using Microsoft.OpenApi.Models;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class MSTitleAttribute(string title) : MSBaseAttribute
{
    // ReSharper disable once MemberCanBePrivate.Global
    public string Title { get; } = title;

    public override void ApplyProperty(OpenApiSchema schema)
    {
        base.ApplyProperty(schema);
        schema.Title = Title;
    }
}