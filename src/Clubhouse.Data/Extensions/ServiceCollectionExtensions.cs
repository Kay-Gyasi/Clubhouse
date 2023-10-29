using Clubhouse.Data.Repositories;
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
            options.UseSqlServer(config.GetConnectionString("DbConnection"), opts =>
                {
                    opts.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                })
                .AddInterceptors(entityAuditor)
                .EnableSensitiveDataLogging();
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var definedTypes = typeof(ServiceCollectionExtensions).Assembly.DefinedTypes.ToList();
        var repositories = definedTypes.Where(x => 
            x.CustomAttributes.Any(a => a.AttributeType == typeof(RepositoryAttribute))).ToList();

        foreach (var repository in repositories)
        {
            var iRepository = definedTypes
                .FirstOrDefault(x => x.Name.Equals($"I{repository.Name}"));
            if (iRepository is null) continue;

            services.AddScoped(iRepository, repository);
        }

        return services;
    }
}