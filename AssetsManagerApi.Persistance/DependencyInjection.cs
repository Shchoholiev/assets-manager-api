using AssetsManagerApi.Persistance.Db;
using Microsoft.Extensions.DependencyInjection;

namespace AssetsManagerApi.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<CosmosDbContext>();
        return services;
    }
}
