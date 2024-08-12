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
        });
    }

    // public void InitialaizeDatabase()
    // {
    //     if (_isDataInitialaized) return;

    //     using var scope = Services.CreateScope();
    //     var mongodbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

    //     var initialaizer = new DbInitializer(mongodbContext);
    //     initialaizer.InitializeDb();

    //     _isDataInitialaized = true;
    // }
}