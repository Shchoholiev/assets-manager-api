using System.Reflection;
using AssetsManagerApi.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace AssetsManagerApi.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetAssembly(typeof(UserProfile)));

        return services;
    }
}
