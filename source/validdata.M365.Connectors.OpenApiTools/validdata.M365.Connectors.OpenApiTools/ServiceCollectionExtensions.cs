using Microsoft.Extensions.DependencyInjection;

namespace validdata.M365.Connectors.OpenApiTools;

public static class ServiceCollectionExtensions
{
    public static void AddConnectorOpenApiTools(
        this IServiceCollection services,
        string host,
        Action<ISetupOptions> configure)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (string.IsNullOrWhiteSpace(host)) throw new ArgumentNullException(nameof(host));
        
        var internalConfig = new SetupOptions {Host = host};
        configure(internalConfig);
        services.AddSwaggerGen(c =>
        {
            OpenApiSetup.ConfigureSwaggerGen(c, internalConfig);
            internalConfig.SwaggerGenAction?.Invoke(c);
        });
    }
}