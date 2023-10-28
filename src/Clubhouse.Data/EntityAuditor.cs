using Clubhouse.Data.Entities.Base;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Clubhouse.Data;

public class EntityAuditor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EntityAuditor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var dbContext = eventData.Context;
        if (dbContext is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (EntityEntry entry in dbContext.ChangeTracker.Entries()
                     .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified))
        {
            if (entry.Entity is not Entity entity) continue;
            entity.UpdatedAt = DateTime.UtcNow;
        }

        foreach (var entry in dbContext.ChangeTracker.Entries()
                     .Where(x => x.State is EntityState.Deleted))
        {
            if (entry.Entity is not Entity entity) continue;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.IsDeleted = true;
        }
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}