using Microsoft.Extensions.Options;
using MongoDb.Data.Settings;
using MongoDB.Driver;

namespace MongoDb.Data.Context;

public class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IOptions<MongoSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
    
    public async Task<T> GetAsync<T>(long id)
    {
        var collection = GetCollection<T>(typeof(T).Name);
        var filter = Builders<T>.Filter.Eq("UserId", id);
        var result = await collection.FindAsync(filter);
        return result.FirstOrDefault();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync<T>()
    {
        var collection = GetCollection<T>(typeof(T).Name);
        var filter = Builders<T>.Filter.Empty;
        var result = await collection.FindAsync(filter);
        return result.ToList();
    }
    
    public async Task<T> AddAsync<T>(T entity)
    {
        var collection = GetCollection<T>(typeof(T).Name);
        await collection.InsertOneAsync(entity);
        return entity;
    }
    
    public async Task<T> UpdateAsync<T>(T entity)
    {
        var collection = GetCollection<T>(typeof(T).Name);
        var filter = Builders<T>.Filter.Eq("UserId", entity.GetType().GetProperty("UserId")!.GetValue(entity)!.ToString());
        await collection.ReplaceOneAsync(filter, entity);
        return entity;
    }
    
    public async Task DeleteAsync<T>(long id)
    {
        var collection = GetCollection<T>(typeof(T).Name);
        var filter = Builders<T>.Filter.Eq("UserId", id);
        await collection.DeleteOneAsync(filter);
    }
}