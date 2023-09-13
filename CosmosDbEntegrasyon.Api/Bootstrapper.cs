using System.Data;
using EntityFrameworkCosmos.Data.Context;
using Microsoft.EntityFrameworkCore;
using MicrosoftCosmos.Data.Context;
using MicrosoftCosmos.Data.Settings;
using MongoDb.Data.Context;
using MongoDb.Data.Settings;

namespace CosmosDbEntegrasyon.Api;

public static class Bootstrapper
{
    public static void AddCosmosDb<TCosmosContext>(this IServiceCollection services, IConfiguration configuration)
        where TCosmosContext : EntityFrameworkContext
    {
        var connectionString = configuration.GetConnectionString("CosmosContext");
        var databaseName = configuration.GetValue<string>("CosmosSettings:DatabaseName");

        if (connectionString is null || databaseName is null)
            throw new DataException();

        services.AddDbContext<TCosmosContext>(options => options.UseCosmos(connectionString, databaseName));
    }
    
    public static void AddMicrosoftCosmosDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<MicrosoftCosmosContext>();
        
        services.Configure<CosmosSettings>(
            configuration.GetSection("MicrosoftCosmosSettings"));
    }
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<MongoContext>();

        services.Configure<MongoSettings>(
            configuration.GetSection("MongoDbSettings"));
    }

}