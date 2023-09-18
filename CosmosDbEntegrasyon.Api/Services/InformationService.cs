using System.Linq.Expressions;
using System.Reflection.Metadata;
using EntityFrameworkCosmos.Data.Entity;
using EntityFrameworkCosmos.Data.Mappers;
using EntityFrameworkCosmos.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmosDbEntegrasyon.Api.Services;

public class InformationService
{
    private readonly DbContext _context;

    public InformationService(DbContext context)
    {
        _context = context;
    }
    
    private DbSet<Information> GetEntityDbSet()
    {
        return _context.Set<Information>();
    }
    public async Task<Information?> GetAsync(string userId, string companyId)
    {
        var response = await GetEntityDbSet().WithPartitionKey(companyId).FirstOrDefaultAsync(x=>x.UserId == userId);
        
        return response;
    }
    
    public async Task<IEnumerable<Information>> GetAsync(InformationQueryFilterModel queryFilterModel,CancellationToken cancellationToken = default)
    {
        var response = GetEntityDbSet().WithPartitionKey(queryFilterModel.CompanyId).AsQueryable();

        var filteredResponse = QuerySpesification(response, queryFilterModel);
        
        return await filteredResponse.ToListAsync(cancellationToken);
    }

    public async Task<Information> CreateAsync(InformationCreateModel createModel,CancellationToken cancellationToken = default)
    {
        var entity = InformationMapper.ToEntity(createModel);
        
        var response = await GetEntityDbSet().AddAsync(entity,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return response.Entity;
    }
    
    public async Task<Information> UpdateAsync(Information entity,CancellationToken cancellationToken = default)
    {
        var response = GetEntityDbSet().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return response.Entity;
    }
    
    public async Task DeleteAsync(string userId, string companyId,CancellationToken cancellationToken = default)
    {
        var entity = await GetAsync(userId,companyId);
        
        if (entity == null)
            return;
        
        GetEntityDbSet().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    private static IQueryable<Information> QuerySpesification(IQueryable<Information> queryableData, InformationQueryFilterModel queryFilterModel)
    {
        var properties = typeof(InformationQueryFilterModel).GetProperties();
        
        foreach (var property in properties)
        {
            if (Constants.ExcludedProperties.Contains(property.Name))
                continue;
            
            var value = property.GetValue(queryFilterModel);
            
            if (value != null)
            {
                queryableData = ApplyPropertyFilter(queryableData, property.Name, value);
            }
        }

        if (queryFilterModel is { PageNumber: > 0, PageSize: > 0 })
            queryableData = ApplyPagination(queryableData, queryFilterModel);
        

        return queryableData;
    }
        
    private static IQueryable<Information> ApplyPagination(IQueryable<Information> queryableData, InformationQueryFilterModel queryFilterModel)
    {
        return queryableData.Skip(((int)queryFilterModel.PageNumber! - 1) * (int)queryFilterModel.PageSize!).Take((int)queryFilterModel.PageSize);
    }
    
    private static IQueryable<Information> ApplyPropertyFilter(IQueryable<Information> queryableData, string propertyName, object? value)
    {
        var parameter = Expression.Parameter(typeof(Information));
        var property = Expression.Property(parameter, propertyName);
        
        if (property.Type.IsGenericType && property.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var underlyingType = Nullable.GetUnderlyingType(property.Type);
            var convertedValue = value != null ? Convert.ChangeType(value, underlyingType!) : null;

            var hasValueProperty = Expression.Property(property, "HasValue");
            var getValueProperty = Expression.Property(property, "Value");
            var equals = Expression.Equal(getValueProperty, Expression.Constant(convertedValue));
            
            var condition = Expression.AndAlso(hasValueProperty, equals);
            var lambda = Expression.Lambda<Func<Information, bool>>(condition, parameter);

            return queryableData.Where(lambda);
        }
        else
        {
            var equals = Expression.Equal(property, Expression.Constant(value));
            var lambda = Expression.Lambda<Func<Information, bool>>(equals, parameter);

            return queryableData.Where(lambda);
        }
    }
}