﻿using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using MicrosoftCosmos.Data.Settings;

namespace MicrosoftCosmos.Data.Context;

//implementation of microsoft cosmos
public class MicrosoftCosmosContext  
{
    private readonly Container _container;
    
    public MicrosoftCosmosContext(IOptions<CosmosSettings> cosmosSettings, string containerName)
    {
        var cosmosClient = new CosmosClient(cosmosSettings.Value.ConnectionString);
        _container = cosmosClient.GetContainer(cosmosSettings.Value.DatabaseName, containerName);
    }
    
    public async Task<T> GetAsync<T>(string id, string partitionKey) where T : class
    {
        var response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
        
        return response.Resource;
    }
    
    public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
    {
        var query = _container.GetItemLinqQueryable<T>().AsQueryable().ToList();
        var sqlTypeQueryResponse = _container.GetItemQueryIterator<T>("SELECT * FROM c WHERE c.id = @id");


        FeedResponse<T>? response = null;
        
        while (sqlTypeQueryResponse.HasMoreResults)
        {
           response = await sqlTypeQueryResponse.ReadNextAsync();
        }

        var list = response?.ToList();

        return query ?? list  ?? new List<T>();
    }
    
    public async Task<T> AddAsync<T>(T entity) where T : class
    {
        var response = await _container.CreateItemAsync(entity, new PartitionKey(entity.GetType().GetProperty("Id")!.GetValue(entity)!.ToString()));
        return response.Resource;
    }
    
    public async Task<T> UpdateAsync<T>(T entity) where T : class
    {
        var response = await _container.UpsertItemAsync(entity);
        return response.Resource;
    }
    
    public async Task DeleteAsync<T>(string id, string partitionKey) where T : class
    {
        await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
    }
}