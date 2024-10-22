using System.Reflection;
using Domain.Aggregates;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class RatDbContext : DbContext, IRatDbContext
{
    public RatDbContext() { }
    public RatDbContext(DbContextOptions<RatDbContext> options) : base(options) { }
    
    public virtual DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RatDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (Environment.GetEnvironmentVariable("CONNECTION_STRING") is not null)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("CONNECTION_STRING"));
        }
        
        base.OnConfiguring(optionsBuilder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        HandleAuditData();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        HandleAuditData();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void HandleAuditData()
    {
        var entries = ChangeTracker.Entries<IBaseAuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Created = DateTime.Now;
                entry.Entity.LastModified = DateTime.Now;
                return;
            }
            
            entry.Entity.LastModified = DateTime.Now;
        }
    }
}