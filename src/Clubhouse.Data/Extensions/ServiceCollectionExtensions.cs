using Clubhouse.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clubhouse.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, 
        IConfiguration config)
    {
        services.AddRepositories();

        services.AddScoped<EntityAuditor>();
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var entityAuditor = sp.GetRequiredService<EntityAuditor>();
            options.UseSqlServer(config.GetConnectionString("DbConnection"))
                .AddInterceptors(entityAuditor);
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {

        return services;
    }
}