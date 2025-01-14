using System.Linq.Expressions;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Persistance.Db;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace AssetsManagerApi.Persistance.Repositories;

public class BaseRepository<TEntity>(CosmosDbContext db, string containerName) 
    : IBaseRepository<TEntity> where TEntity : EntityBase
{
    protected CosmosDbContext _db = db;
    
    protected Container _container = db.GetContainerAsync(containerName).Result;

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        entity.Id = Guid.NewGuid().ToString();
        var response = await _container.CreateItemAsync(entity, cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var response = await _container.DeleteItemAsync<TEntity>(
            entity.Id, 
            new PartitionKey(entity.Id), 
            cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        var iterator = _container.GetItemLinqQueryable<TEntity>()
            .Where(predicate)
            .Take(1)
            .ToFeedIterator();

        var response = await iterator.ReadNextAsync(cancellationToken);
        return response.Count != 0;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        var query = "SELECT VALUE COUNT(1) FROM c";
        var queryDefinition = new QueryDefinition(query);
        var queryResultSetIterator = _container.GetItemQueryIterator<int>(queryDefinition);

        var currentResultSet = await queryResultSetIterator.ReadNextAsync(cancellationToken);
        
        return currentResultSet.Resource.FirstOrDefault();
    }

    public async Task<int> GetCountAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken)
    {
        var countQuery = _container.GetItemLinqQueryable<TEntity>()
            .Where(predicate)
            .Select(_ => 1) 
            .ToFeedIterator();

        var count = 0;
        while (countQuery.HasMoreResults)
        {
            var response = await countQuery.ReadNextAsync(cancellationToken);
            count += response.Resource.Count();
        }

        return count;
    }
    public async Task<TEntity> GetOneAsync(string id, CancellationToken cancellationToken)
    {
        var response = await _container.ReadItemAsync<TEntity>(
            id, 
            new PartitionKey(id), 
            cancellationToken: cancellationToken);
        return response.Resource;
    }

    public async Task<TEntity> GetOneAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken)
    {
        var query = _container.GetItemLinqQueryable<TEntity>()
            .Where(predicate)
            .Take(1)
            .ToFeedIterator();

        var response = await query.ReadNextAsync(cancellationToken);
        return response.FirstOrDefault();
    }

    public async Task<List<TEntity>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _container.GetItemLinqQueryable<TEntity>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToFeedIterator();

        var response = await query.ReadNextAsync(cancellationToken);
        return response.ToList();
    }

    public async Task<List<TEntity>> GetPageAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        var query = _container.GetItemLinqQueryable<TEntity>()
            .Where(predicate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToFeedIterator();

        var response = await query.ReadNextAsync(cancellationToken);
        return response.ToList();
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        var query = _container.GetItemLinqQueryable<TEntity>()
            .Where(predicate)
            .ToFeedIterator();

        var response = await query.ReadNextAsync(cancellationToken);
        return response.ToList();
    }
}
