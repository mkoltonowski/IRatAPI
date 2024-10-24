using System.Reflection;
using Domain.Aggregates;
using Domain.Aggregates.Account;
using Domain.Aggregates.Identity;
using Domain.Shared;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public class RatDbContext : IdentityDbContext, IRatDbContext
{
    public RatDbContext() { }
    public RatDbContext(DbContextOptions<RatDbContext> options) : base(options) { }
    
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<ClientUser> ClientUsers { get; set; }
    public virtual DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RatDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
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