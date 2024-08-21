using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace AssetsManagerApi.Persistance.Db;

public class CosmosDbContext
{
    private readonly CosmosClient _cosmosClient;

    private readonly Database _database;

    public CosmosDbContext(IConfiguration configuration)
    {
        var cosmosDbSettings = configuration.GetSection("CosmosDb");
        var endpointUri = cosmosDbSettings["Endpoint"]!;
        var primaryKey = cosmosDbSettings["PrimaryKey"]!;
        var databaseId = cosmosDbSettings["DatabaseName"]!;

        _cosmosClient = new CosmosClient(endpointUri, primaryKey);
        _database = _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
    }

    public async Task<Container> GetContainerAsync(string containerId)
    {
        return await _database.CreateContainerIfNotExistsAsync(containerId, "/id");
    }

    public void Dispose()
    {
        _cosmosClient?.Dispose();
    }
}

