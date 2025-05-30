using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Infrastructure.Services;
using AssetsManagerApi.Persistance.Db;
using AssetsManagerApi.Persistance.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AssetsManagerApi.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<CosmosDbContext>();

        services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
        services.AddScoped<IRolesRepository, RolesRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ICodeAssetsRepository, CodeAssetsRepository>();
        services.AddScoped<ITagsRepository, TagsRepository>();
        services.AddScoped<IFoldersRepository, FoldersRepository>();
        services.AddScoped<ICodeFilesRepository, CodeFilesRepository>();
        services.AddScoped<ICompaniesRepository, CompaniesRepository>();

        services.AddScoped<IStartProjectsRepository, StartProjectsRepository>();

        return services;
    }
}
