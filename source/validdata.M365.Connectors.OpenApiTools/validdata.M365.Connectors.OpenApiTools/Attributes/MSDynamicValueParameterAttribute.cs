using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class MSDynamicValueParameterAttribute(
    string parameter,
    string operationId,
    string valuePath,
    string valueTitle,
    bool requiresTenant = false)
    : MSBaseAttribute
{
    // ReSharper disable MemberCanBePrivate.Global
    public string Parameter { get; } = parameter;
    public string OperationId { get; } = operationId;
    public string ValuePath { get; } = valuePath;
    public string ValueTitle { get; } = valueTitle;
    public bool RequiresTenant { get; } = requiresTenant; // TODO: Maybe default to true later on? Probably most endpoints will need a tenant...  
    // ReSharper restore MemberCanBePrivate.Global

    public override void ApplyOperation(OperationFilterContext context, OpenApiOperation operation)
    {
        var apiParameter = operation.Parameters.FirstOrDefault(x =>
            string.Equals(x.Name, Parameter, StringComparison.InvariantCultureIgnoreCase));
        if (apiParameter == null) throw new Exception($"Parameter {Parameter} not found");

        var dynamicExtension = new OpenApiObject
        {
            ["operationId"] = new OpenApiString(OperationId),
            ["value-path"] = new OpenApiString(ValuePath.LowerFirstChar()),
            ["value-title"] = new OpenApiString(ValueTitle.LowerFirstChar())
        };

        if (RequiresTenant)
        {
            dynamicExtension["parameters"] = new OpenApiObject
            {
                ["tenantId"] = new OpenApiObject
                {
                    ["parameter"] = new OpenApiString("tenantId"),
                }
            };
        }

        apiParameter.Extensions.Add("x-ms-dynamic-values", dynamicExtension);
    }
}