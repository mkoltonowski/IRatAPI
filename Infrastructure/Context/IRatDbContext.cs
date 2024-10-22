using Domain.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public interface IRatDbContext
{
    DbSet<Message> Messages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}