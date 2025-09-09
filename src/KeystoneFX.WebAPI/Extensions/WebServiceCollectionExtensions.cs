using KeystoneFX.Application.Extensions.DependencyInjection;
using KeystoneFX.Infrastructure.Extensions.DependencyInjection;

namespace KeystoneFX.WebAPI.Extensions;

public static class WebServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .ApplicationDependencies()
            .InfrastructureDependencies(configuration);

        // Register Services
        services.AddAuthentication();
        services.AddHttpContextAccessor();

        return services;
    }
}