using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Kernel.Interfaces;

namespace Shared.Infrastructure;

public static class DbContextExtensions
{
    public static void ApplyAuditableEntities(this ChangeTracker changeTracker)
    {
        var entries = changeTracker.Entries<IAuditableEntity>();
        var now = DateTime.UtcNow;
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }

            if (entry.State == EntityState.Modified && !entry.Property(e => e.UpdatedAt).IsModified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}