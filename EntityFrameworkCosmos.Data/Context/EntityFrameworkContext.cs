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

    public virtual DbSet<Information> Session { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Information>().ToContainer(nameof(Session)).HasPartitionKey(x => x.UserId);
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