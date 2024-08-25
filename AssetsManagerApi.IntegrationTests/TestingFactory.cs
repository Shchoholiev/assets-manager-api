using AssetsManagerApi.Persistance.Db;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssetsManagerApi.IntegrationTests;

public class TestingFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    private bool _isDataInitialaized = false;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // context.HostingEnvironment.EnvironmentName = "Test";

            config
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Test.json", optional: true, reloadOnChange: true);

            config.AddEnvironmentVariables();
        });
    }

    public void InitialaizeDatabase()
    {
        if (_isDataInitialaized) return;

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CosmosDbContext>();

        var initialaizer = new DbInitializer(dbContext);
        initialaizer.InitializeDb();

        _isDataInitialaized = true;
    }
}