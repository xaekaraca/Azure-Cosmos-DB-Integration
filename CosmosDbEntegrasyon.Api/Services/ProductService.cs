using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using MicrosoftCosmos.Data.Entity;
using MicrosoftCosmos.Data.Mapper;
using MicrosoftCosmos.Data.Models;
using MicrosoftCosmos.Data.Settings;
using static System.Text.RegularExpressions.Regex;

namespace CosmosDbEntegrasyon.Api.Services;

public class ProductService
{
    private readonly Container _container;

    public ProductService(IOptions<CosmosSettings> cosmosSettings)
    {
        var cosmosClient = new CosmosClient(cosmosSettings.Value.ConnectionString);
        _container = cosmosClient.GetContainer(cosmosSettings.Value.DatabaseName, Constants.ProductsContainerName);
        _container.Database.CreateContainerIfNotExistsAsync( Constants.ProductsContainerName, "/CategoryId");
    }

    public async Task<Product> GetAsync(string id, string categoryId)
    {
        var response = await _container.ReadItemAsync<Product>(id, new PartitionKey(categoryId));

        return response.Resource;
    }

    public async Task<(IEnumerable<Product> Results, string ContinuationToken)> GetAllWithLinqAsync(ProductQueryFilterModel queryFilterModel, CancellationToken cancellationToken = default)
    {
        var queryOptions = new QueryRequestOptions
        {
            PartitionKey = queryFilterModel.CategoryId == null ? PartitionKey.None : new PartitionKey(queryFilterModel.CategoryId),
            MaxItemCount = queryFilterModel.PageSize,
        };

        if (queryFilterModel.ContinuationToken!= null)
            queryFilterModel.ContinuationToken = Unescape(queryFilterModel.ContinuationToken);
        
        var query = _container.GetItemLinqQueryable<Product>(false, queryFilterModel.ContinuationToken , queryOptions);

        query = QuerySpesification(query, queryFilterModel) as IOrderedQueryable<Product>;

        var iterator = query.ToFeedIterator();
        var results = new List<Product>();
    
        var newContinuationToken = string.Empty;
        
        if (iterator.HasMoreResults)
        {
            var currentResultSet = await iterator.ReadNextAsync(cancellationToken);
            newContinuationToken = currentResultSet.ContinuationToken;
            results.AddRange(currentResultSet);
        }

        return (results, newContinuationToken);
    }

    public async Task<IEnumerable<Product>> GetAllWithIteratorAsync(string? categoryId,CancellationToken cancellationToken = default)
    {
        var sqlTypeQueryResponse = _container.GetItemQueryIterator<Product>("SELECT * FROM Products", default, new QueryRequestOptions
        {
            PartitionKey = categoryId == null ? PartitionKey.None : new PartitionKey(categoryId),
        });
        var products = new List<Product>();

        while (sqlTypeQueryResponse.HasMoreResults)
        {
            var response = await sqlTypeQueryResponse.ReadNextAsync(cancellationToken);
            products.AddRange(response.ToList());
        }

        return products;
    }
    
    public async Task<Product> CreateAsync(ProductCreateModel productCreateModel, CancellationToken cancellationToken = default)
    {
        var product = ProductMapper.ToEntity(productCreateModel);

        var response = await _container.CreateItemAsync(product, new PartitionKey(product.CategoryId), cancellationToken: cancellationToken);

        return response.Resource;
    }
    
    public async Task<Product> UpdateAsync(string id, string categoryId, ProductUpdateModel productUpdateModel, CancellationToken cancellationToken = default)
    {
        var product = await GetAsync(id, categoryId);

        product = productUpdateModel.ToEntity(product);
        product.ChangedAt = DateTime.UtcNow;

        var response = await _container.ReplaceItemAsync(product, id, new PartitionKey(categoryId), cancellationToken: cancellationToken);

        return response.Resource;
    }
    
    public async Task DeleteAsync(string id, string categoryId, CancellationToken cancellationToken = default)
    {
        var product = await GetAsync(id, categoryId);
        
        product.IsDeleted = true;
        product.ChangedAt = DateTime.UtcNow;
        
        await _container.ReplaceItemAsync(product, id, new PartitionKey(categoryId),cancellationToken: cancellationToken);
    }
    
    private static IQueryable<Product> QuerySpesification(IQueryable<Product> query, ProductQueryFilterModel queryFilterModel)
    {
        var properties = typeof(ProductQueryFilterModel).GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(queryFilterModel);

            if (value == null)
                continue;

            if (Constants.ExcludedProperties.Contains(property.Name))
                continue;

            query = ApplyPropertyFilter(query,property.Name, value);
        }
        
        query = ApplyPriceFilter(query, queryFilterModel.MinPrice, queryFilterModel.MaxPrice);

        return query;
    }

    private static IQueryable<Product> ApplyPriceFilter(IQueryable<Product> query, decimal? minPrice, decimal? maxPrice)
    {
        if (minPrice == null && maxPrice == null)
            return query;
        
        var parameter = Expression.Parameter(typeof(Product), "p");
        
        var property = Expression.Property(parameter, "Price");
        var propertyType = property.Type;
        
        
        if (minPrice != null)
        {
            var minPriceProperty = Expression.Constant(minPrice);
            var minPriceConverted = Expression.Convert(minPriceProperty, propertyType);
            var minPriceExpression = Expression.GreaterThanOrEqual(property, minPriceConverted);
            
            var minPriceLambda = Expression.Lambda<Func<Product, bool>>(minPriceExpression, parameter);
            
            query = query.Where(minPriceLambda);
        }

        if (maxPrice == null) return query;
        
        var maxPriceProperty = Expression.Constant(maxPrice);
        var maxPriceConverted = Expression.Convert(maxPriceProperty, propertyType);
        var maxPriceExpression = Expression.LessThanOrEqual(property, maxPriceConverted);
            
        var maxPriceLambda = Expression.Lambda<Func<Product, bool>>(maxPriceExpression, parameter);
            
        query = query.Where(maxPriceLambda);

        return query;
    }

    private static IQueryable<Product> ApplyPropertyFilter (IQueryable<Product> query,string propertyName, object value)
    {
        var parameter = Expression.Parameter(typeof(Product), "p");
        var property = Expression.Property(parameter, propertyName);
        
        var propertyType = property.Type;
        var constant = Expression.Constant(value);
        
        var convertedConstant = Expression.Convert(constant, propertyType);
        
        var expression = Expression.Equal(property, convertedConstant);
        var lambda = Expression.Lambda<Func<Product, bool>>(expression, parameter);

        return query.Where(lambda);
    }
}