namespace KeystoneFX.WebAPI.Extensions;

public static class WebServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IHostBuilder host, IConfiguration configuration)
    {

        // Register Services
        services.AddAuthentication();
        services.AddHttpContextAccessor();

        return services;
    }
}