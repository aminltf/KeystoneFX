using KeystoneFX.Infrastructure.Persistence.Specifications;
using KeystoneFX.Infrastructure.Persistence.Repositories;
using KeystoneFX.Infrastructure.Persistence.UnitOfWork;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using Microsoft.Extensions.DependencyInjection;
using KeystoneFX.Shared.Kernel.Abstractions.Time;
using KeystoneFX.Shared.Kernel.Abstractions.Identity;
using KeystoneFX.Infrastructure.Security;
using KeystoneFX.Infrastructure.Interceptors;
using KeystoneFX.Infrastructure.Identity.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using KeystoneFX.Application.Common.Abstractions.Repositories.Identity;
using KeystoneFX.Infrastructure.Identity.Repositories;
using KeystoneFX.Application.Common.Abstractions.Contexts;

namespace KeystoneFX.Infrastructure.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection InfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // IdentityContext
        services.AddDbContext<IdentityContext>((sp, opts) =>
        {
            opts.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
            opts.AddInterceptors(
                sp.GetRequiredService<AuditingInterceptor<Guid>>(),
                sp.GetRequiredService<SoftDeleteInterceptor<Guid>>());
        });
        services.AddScoped<IIdentityContext>(provider => provider.GetRequiredService<IdentityContext>());

        // Providers
        services.AddSingleton<IClock>(new SystemClock());

        // Security
        services.AddScoped<ICurrentUser<Guid>, HttpContextCurrentUser<Guid>>();

        // Generic Interceptors
        services.AddScoped<AuditingInterceptor<Guid>>();
        services.AddScoped<SoftDeleteInterceptor<Guid>>();

        // Repositories
        services.AddScoped<IUserWriteRepository, UserWriteRepository>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IRoleWriteRepository, RoleWriteRepository>();
        services.AddScoped<IRoleReadRepository, RoleReadRepository>();
        services.AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>();
        services.AddScoped(typeof(IReadRepository<,>), typeof(EfReadRepository<,>));
        services.AddScoped(typeof(IWriteRepository<,>), typeof(EfWriteRepository<,>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddSingleton(new SpecificationEvaluatorOptions
        {
            UseSplitQueryForMultipleIncludes = true,
            ApplyIncludesInProjection = false
        });

        return services;
    }
}