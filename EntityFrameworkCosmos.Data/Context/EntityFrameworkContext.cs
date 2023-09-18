using EntityFrameworkCosmos.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCosmos.Data.Context;

public class EntityFrameworkContext : DbContext
{
    protected EntityFrameworkContext()
    {
        
    }
    
    protected EntityFrameworkContext(DbContextOptions options) : base(options)
    {
        
    }

    public virtual DbSet<Information> Information { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Information>().ToContainer(nameof(Information)).HasPartitionKey(x => x.CompanyId);
    }
    
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnBeforeSaveChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        OnBeforeSaveChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected virtual void OnBeforeSaveChanges()
    {
        OnBeforeAddedEntities();
        OnBeforeModifiedEntities();
        OnBeforeDeletedEntities();
    }

    protected virtual void OnBeforeDeletedEntities()
    {
        var deletedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).ToList();
        foreach (var item in deletedEntities.Where(item => item.Entity.GetType().GetProperty("IsDeleted") != null))
        {
            item.State = EntityState.Modified;
            item.Property("IsDeleted").CurrentValue = true;
        }
    }

    protected virtual void OnBeforeAddedEntities()
    {
        var addedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
        foreach (var item in addedEntities)
        {
            if (item.Entity.GetType().GetProperty("CreatedAt") != null)
                item.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
            if (item.Entity.GetType().GetProperty("ChangedAt") != null)
                item.Property("ChangedAt").CurrentValue = item.Property("CreatedAt").CurrentValue;
        }
    }

    protected virtual void OnBeforeModifiedEntities()
    {
        var editedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();
        foreach (var item in editedEntities)
        {
            if (item.Entity.GetType().GetProperty("CreatedAt") != null)
                item.Property("CreatedAt").CurrentValue = (DateTime)(item.Property("CreatedAt").OriginalValue ?? DateTime.MinValue) == DateTime.MinValue ? DateTime.UtcNow : item.Property("CreatedAt").OriginalValue;

            if (item.Entity.GetType().GetProperty("ChangedAt") != null)
                item.Property("ChangedAt").CurrentValue = DateTime.UtcNow;
        }
    }
}