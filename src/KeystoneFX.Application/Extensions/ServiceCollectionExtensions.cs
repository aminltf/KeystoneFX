using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KeystoneFX.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ApplicationDependencies(this IServiceCollection services)
    {
        // Register Commands and Queries
        services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register Mappings
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}