using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace validdata.M365.Connectors.OpenApiTools;

internal class OpenApiSetup
{
    internal static void ConfigureSwaggerGen(
        SwaggerGenOptions options,
        SetupOptions setupOptions)
    {
        if (setupOptions == null)
        {
            throw new Exception("Configuration section \"ConnectorOpenApiConfiguration\" is missing");
        }

        options.SwaggerDoc("connector", new OpenApiInfo
        {
            Title = setupOptions.Title,
            Description = setupOptions.Description,
            Version = setupOptions.Version,
        });
        
        if (setupOptions.AuthenticateWithAzureAdDefaultsEnabled)
        {
            SetupDefaultAzureAdAuthentication(options);
        }
        
        options.OperationFilter<OpenApiOperationFilter>();
        options.DocumentFilter<OpenApiDocumentFilter>(setupOptions);

        RegisterSpecialBodyForWebhookOperations(options);
        
        if (setupOptions.XmlCommentsAssembly != null)
        {
            var xmlFilename = $"{setupOptions.XmlCommentsAssembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));    
        }
        
    }

    private static void RegisterSpecialBodyForWebhookOperations(SwaggerGenOptions options)
    {
        options.MapType<WebhookRegistrationRequest>(() => new OpenApiSchema
        {
            Type = "object",
            Description = "webhook create request body",
            Required = new HashSet<string> { nameof(WebhookRegistrationRequest.Url).LowerFirstChar() },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                [nameof(WebhookRegistrationRequest.Url).LowerFirstChar()] = new()
                {
                    Type = "string",
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        ["x-ms-notification-url"] = new OpenApiBoolean(true),
                        ["x-ms-visibility"] = new OpenApiString("internal")
                    }
                }
            }
        });
    }

    private static void SetupDefaultAzureAdAuthentication(SwaggerGenOptions options)
    {
        var openApiSecurityScheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows()
            {
                AuthorizationCode = new OpenApiOAuthFlow()
                {
                    AuthorizationUrl = new Uri("https://login.microsoftonline.com/common/oauth2/authorize"),
                    TokenUrl = new Uri("https://login.windows.net/common/oauth2/authorize")
                },
                Extensions = new Dictionary<string, IOpenApiExtension>()
                {
                    ["scopes"] = new OpenApiObject()
                    {
                        ["User.Read"] = new OpenApiString("User.Read")
                    }
                }
            },
            BearerFormat = "bearer",
            Scheme = "bearer",
        };
        options.AddSecurityDefinition("azure-ad-default", openApiSecurityScheme);
        
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "azure-ad-default"
                }
            }] = new List<string>()
        });
        
    }
}